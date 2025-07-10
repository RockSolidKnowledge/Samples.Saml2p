using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using Rsk.Saml;
using Rsk.Saml.IdentityProvider.Storage.EntityFramework;
using Rsk.Saml.IdentityProvider.Storage.EntityFramework.DbContexts;
using Rsk.Saml.IdentityProvider.Storage.EntityFramework.Mappers;
using Rsk.Saml.Models;
using Rsk.Saml.OpenIddict.EntityFrameworkCore.DbContexts;
using openiddictidp.Data;
using System;
using System.Data;
using System.Data.Common;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using static OpenIddict.Abstractions.OpenIddictConstants;
using ServiceProvider = Rsk.Saml.Models.ServiceProvider;
using Microsoft.Data.SqlClient;
using MySqlConnector;
using Npgsql;

namespace openiddictidp;

public class Worker(IServiceProvider serviceProvider, IConfiguration configuration) : IHostedService
{
    private DbProvider dbProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        dbProvider = configuration.GetValue<DbProvider>("DbProvider");

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();
        await EnsureAllDatabasesAreCreated(scope);
        await CreateMvcClientIfNotExists(scope);
        await CreateSamlClientIfNotExists(scope);
        await CreateFakeUserBobIfNotExists(scope);
        await CreateEmailScopeIfNotExists(scope);
        await CreateServiceProviderIfNotExists(scope, "https://localhost:5001/saml/artifact",
            x =>
            {
                x.EntityId = "https://localhost:5001/saml/artifact";
                x.EncryptAssertions = false;
                x.AssertionConsumerServices.Add(new Service(SamlConstants.BindingTypes.HttpArtifact,
                    "https://localhost:5001/signin-saml-openIddict-artifact"));
                x.SingleLogoutServices.Add(new Service(SamlConstants.BindingTypes.HttpRedirect,
                    "https://localhost:5001/signout-saml-artifasct"));
                x.ArtifactResolutionServices.Add(new Service(SamlConstants.BindingTypes.Soap,
                    "https://localhost:5001/ars-saml"));
                x.SigningCertificates = [new X509Certificate2("Resources/testclient.cer")];
                x.EncryptionCertificate = new X509Certificate2("Resources/idsrv3test.cer");
            });
        await CreateServiceProviderIfNotExists(scope, "https://localhost:5001/saml", x =>
        {
            x.EntityId = "https://localhost:5001/saml";
            x.EncryptAssertions = false;
            x.AllowIdpInitiatedSso = true;
            x.AssertionConsumerServices.Add(new Service(SamlConstants.BindingTypes.HttpPost,
                "https://localhost:5001/signin-saml-openIddict"));
            x.SingleLogoutServices.Add(new Service(SamlConstants.BindingTypes.HttpRedirect,
                "https://localhost:5001/signout-saml"));
            x.SigningCertificates = [new X509Certificate2("Resources/testclient.cer")];
            x.EncryptionCertificate = new X509Certificate2("Resources/idsrv3test.cer");
        });
        await SaveAllChanges(scope);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task EnsureAllDatabasesAreCreated(IServiceScope scope)
    {
        // Apply migrations for the ApplicationDbContext.
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await applicationDbContext.Database.MigrateAsync();

        // Apply migrations for the OpenIddictSamlMessageDbContext.
        var samlOpenIddictMessageContext = scope.ServiceProvider.GetRequiredService<OpenIddictSamlMessageDbContext>();
        await samlOpenIddictMessageContext.Database.MigrateAsync();

        // Apply migrations for the SamlConfigurationDbContext.
        var samlConfigurationContext = scope.ServiceProvider.GetRequiredService<SamlConfigurationDbContext>();
        await samlConfigurationContext.Database.MigrateAsync();

        //Create the database used by the quartz scheduler.
        await CreateQuartzDatabase(scope);
    }

    private async Task CreateQuartzDatabase(IServiceScope scope)
    {
        const string quartzDatabaseDownloadUrlTemplate = "https://raw.githubusercontent.com/quartznet/quartznet/refs/heads/main/database/tables/tables_{0}.sql";
        string quartzDatabaseDownloadUrl = string.Format(quartzDatabaseDownloadUrlTemplate, GetDbNameForQuartzTablesSqlUrl());
        string quartzDatabaseSql = await DownloadQuartzDatabaseSql(scope, quartzDatabaseDownloadUrl);
        await InitializeQuartzDatabase(scope, quartzDatabaseSql);
    }

    private string GetDbNameForQuartzTablesSqlUrl() => dbProvider switch
    {
        DbProvider.SqlServer => "sqlServer",
        DbProvider.MySql => "mysql_innodb",
        DbProvider.PostgreSql => "postgres",
        DbProvider.Sqlite => "sqlite",
        _ => throw new NotSupportedException($"The database provider {dbProvider} is not supported.")
    };

    private static async Task<string> DownloadQuartzDatabaseSql(IServiceScope scope, string quartzDatabaseDownloadUrl)
    {
        var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        using HttpClient client = clientFactory.CreateClient();
        return await client.GetStringAsync(quartzDatabaseDownloadUrl);
    }

    private async Task InitializeQuartzDatabase(IServiceScope scope, string quartzDatabaseSql)
    {
        using DbConnection connection = GetDbConnection(scope);
        using DbCommand command = connection.CreateCommand();
        command.CommandType = CommandType.Text;
        await connection.OpenAsync();
        command.CommandText = quartzDatabaseSql;
        await command.ExecuteNonQueryAsync();
    }
    
    private DbConnection GetDbConnection(IServiceScope scope)
    {
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        return dbProvider switch
        {
            DbProvider.SqlServer => new SqlConnection(connectionString),
            DbProvider.MySql => new MySqlConnection(connectionString),
            DbProvider.PostgreSql => new NpgsqlConnection(connectionString),
            DbProvider.Sqlite => new SqliteConnection(connectionString),
            _ => throw new NotSupportedException($"The database provider {dbProvider} is not supported.")
        };
    }

    private static Task CreateMvcClientIfNotExists(IServiceScope scope) => CreateClientIfNotExists(scope, "mvc", ocd =>
    {
        ocd.ClientId = "mvc";
        ocd.ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654";
        ocd.ConsentType = ConsentTypes.Explicit;
        ocd.DisplayName = "MVC client application";
        ocd.RedirectUris.Add(new Uri("https://localhost:44338/callback/login/local"));
        ocd.PostLogoutRedirectUris.Add(new Uri("https://localhost:44338/callback/logout/local"));

        ocd.Permissions.UnionWith([
            Permissions.Endpoints.Authorization,
            Permissions.Endpoints.EndSession,
            Permissions.Endpoints.Token,
            Permissions.GrantTypes.AuthorizationCode,
            Permissions.ResponseTypes.Code,
            Permissions.Scopes.Email,
            Permissions.Scopes.Profile,
            Permissions.Scopes.Roles
        ]);
        ocd.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);
    });

    private static Task CreateSamlClientIfNotExists(AsyncServiceScope scope) => CreateClientIfNotExists(scope, "https://localhost:5001/saml", x =>
    {
        x.ClientId = "https://localhost:5001/saml";
        x.Permissions.UnionWith([Permissions.Scopes.Email]);
    });

    private static async Task CreateClientIfNotExists(IServiceScope scope, string clientId,
        Action<OpenIddictApplicationDescriptor> descriptorCoonfiguration)
    {
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        if (await manager.FindByClientIdAsync(clientId) == null)
        {
            OpenIddictApplicationDescriptor newClientDescriptor =
                CreateAndConfigureOpenIddictApplicationDescriptor(descriptorCoonfiguration);
            await manager.CreateAsync(newClientDescriptor);
        }
    }

    private static OpenIddictApplicationDescriptor CreateAndConfigureOpenIddictApplicationDescriptor(
        Action<OpenIddictApplicationDescriptor> configuration)
    {
        var od = new OpenIddictApplicationDescriptor();
        configuration(od);
        return od;
    }

    private static Task CreateFakeUserBobIfNotExists(IServiceScope scope) => CreateUserIfNotExists(scope, "bob@test.fake", user =>
    {
        user.UserName = "bob@test.fake";
        user.Email = "bob@test.fake";
    }, "Password123!");

    private static async Task CreateUserIfNotExists(IServiceScope scope, string userName,
        Action<ApplicationUser> userConfiguration, string userPassword)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (await userManager.FindByNameAsync(userName) == null)
        {
            var user = new ApplicationUser();
            userConfiguration(user);
            await userManager.CreateAsync(user, userPassword);
        }
    }

    private static Task CreateEmailScopeIfNotExists(IServiceScope scope)
    {
        var claims = new[] { "email" };
        string serializedClaims = JsonSerializer.Serialize(claims);
        using JsonDocument jsonDocument = JsonDocument.Parse(serializedClaims);
        JsonElement claimsElemennt = jsonDocument.RootElement.Clone();
        return CreateScopeIfNotExists(scope, "email", x =>
        {
            x.Name = "email";
            x.Resources.Add("https://localhost:5001/saml");
            x.Properties.Add("Claims", claimsElemennt);
        });
    }

    private static async Task CreateScopeIfNotExists(IServiceScope scope, string scopeName,
        Action<OpenIddictScopeDescriptor> scopeConfiguration)
    {
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        if (await scopeManager.FindByNameAsync(scopeName) == null)
        {
            var scopeDescriptor = new OpenIddictScopeDescriptor();
            scopeConfiguration(scopeDescriptor);

            await scopeManager.CreateAsync(scopeDescriptor);
        }
    }

    private static async Task CreateServiceProviderIfNotExists(IServiceScope scope, string entityId,
        Action<ServiceProvider> serviceProviderConfiguration)
    {
        var samlConfigurationDbContext = scope.ServiceProvider.GetRequiredService<ISamlConfigurationDbContext>();

        if (await samlConfigurationDbContext.ServiceProviders.SingleOrDefaultAsync(x => x.EntityId == entityId) == null)
        {
            var serviceProvider = new ServiceProvider();
            serviceProviderConfiguration(serviceProvider);
            samlConfigurationDbContext.ServiceProviders.Add(serviceProvider.ToEntity());
        }
    }

    private static async Task SaveAllChanges(IServiceScope scope)
    {
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await applicationDbContext.SaveChangesAsync();
        var samlConfigurationDbContext = scope.ServiceProvider.GetRequiredService<ISamlConfigurationDbContext>();
        await samlConfigurationDbContext.SaveChangesAsync();
    }
}

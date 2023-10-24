using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using static OpenIddict.Abstractions.OpenIddictConstants;
using ServiceProvider = Rsk.Saml.Models.ServiceProvider;

namespace openiddictidp;

public class Worker : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    public Worker(IServiceProvider serviceProvider)
        => _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        await EnsureAllDatabasesAreCreated(scope);
        await CreateMvcClientIfNotExists(scope);
        await CreateSamlClientIfNotExists(scope);
        await CreateFakeUserBobIfNotExists(scope);
        await CreateEmailScopeIfNotExists(scope);
        await CreateSamlServiceProviderWithArtifactBindingIfNotExists(scope, "https://localhost:5001/saml/artifact", x =>
        {
            x.EntityId = "https://localhost:5001/saml/artifact";
            x.EncryptAssertions = false;
            x.AssertionConsumerServices.Add(new Service(SamlConstants.BindingTypes.HttpArtifact,
                "https://localhost:5001/signin-saml-openIddict-artifact"));
            x.SingleLogoutServices.Add(new Service(SamlConstants.BindingTypes.HttpRedirect,
                "https://localhost:5001/signout-saml-artifasct"));
            x.ArtifactResolutionServices.Add(new Service(SamlConstants.BindingTypes.Soap, "https://localhost:5001/ars-saml"));
            x.SigningCertificates = new List<X509Certificate2> { new("Resources/testclient.cer") };
            x.EncryptionCertificate = new X509Certificate2("Resources/idsrv3test.cer");
        });
        await CreateServiceProviderIfNotExists(scope, "https://localhost:5001/saml", x =>
        {
            x.EntityId = "https://localhost:5001/saml";
            x.EncryptAssertions = false;
            x.AllowIdpInitiatedSso = true;
            x.AssertionConsumerServices.Add(new Service(SamlConstants.BindingTypes.HttpPost, "https://localhost:5001/signin-saml-openIddict"));
            x.SingleLogoutServices.Add(new Service(SamlConstants.BindingTypes.HttpRedirect, "https://localhost:5001/signout-saml"));
            x.SigningCertificates = new List<X509Certificate2> { new("Resources/testclient.cer") };
            x.EncryptionCertificate = new X509Certificate2("Resources/idsrv3test.cer");
        });
        await SaveAllChanges(scope);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task EnsureAllDatabasesAreCreated(IServiceScope scope)
    {
        //Create the database backed by the ApplicationDbContext.
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await applicationDbContext.Database.EnsureCreatedAsync();

        //Create the database backed by the OpenIddictSamlMessageDbContext.
        var samlOpenIddictMessageContext = scope.ServiceProvider.GetRequiredService<OpenIddictSamlMessageDbContext>();
        await samlOpenIddictMessageContext.Database.EnsureCreatedAsync();

        //Create the database backed by the SamlConfigurationContext.
        var samlConfigurationContext = scope.ServiceProvider.GetRequiredService<SamlConfigurationDbContext>();
        await samlConfigurationContext.Database.EnsureCreatedAsync();
        //Create the database used by the quartz scheduler.
        await CreateTheQuartzDatabase(scope);
    }

    private async Task CreateTheQuartzDatabase(IServiceScope scope)
    {
        var quartzDatabaseDownloadUrlTemplate ="https://github.com/quartznet/quartznet/blob/main/database/tables/tables_{0}.sql";
        string quartzDatabaseDownloadUrl = null;
        quartzDatabaseDownloadUrl = String.Format(quartzDatabaseDownloadUrlTemplate, "sqlServer");
        
        var quartzDatabaseSql =await DownloadQuartzDatabaseSql(scope, quartzDatabaseDownloadUrl);
        
        await InitializeQuartzDatabase(scope, quartzDatabaseSql);
    }

    private static async Task<string> DownloadQuartzDatabaseSql(IServiceScope scope, string quartzDatabaseDownloadUrl)
    {
        var clientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        using var client = clientFactory.CreateClient();
        return await client.GetStringAsync(quartzDatabaseDownloadUrl);
    }

    private async Task InitializeQuartzDatabase(IServiceScope scope, string quartzDatabaseSql)
    {
        var quartzDatabaseCommands =GetQuartzDatabaseCommands(quartzDatabaseSql);
        var connection =GetDbConnection(scope);
        var command = connection.CreateCommand();
        command.CommandType= CommandType.Text;
        await connection.OpenAsync();
        foreach (var quartzDatabaseCommand in quartzDatabaseCommands)
        {
            command.CommandText = quartzDatabaseCommand;
            await command.ExecuteNonQueryAsync();
        }

        await connection.DisposeAsync();
    }

    private static IList<string> GetQuartzDatabaseCommands(string quartzDatabaseSql)
    {
        var databaseDelimiter = GetDatabaseDelimiter();
        return quartzDatabaseSql.Split(databaseDelimiter).Select(x => x.Trim()).ToList();
    }

    private static string GetDatabaseDelimiter()
    {
        return "GO";
    }
    
    private static DbConnection GetDbConnection(IServiceScope scope)
    {
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        DbConnection connection = null;
        connection = new SqlConnection(connectionString);
        return connection;
    }

    private Task CreateMvcClientIfNotExists(IServiceScope scope)
    {
        return CreateClientIfNotExists(scope, "mvc", ocd =>
        {
            ocd.ClientId = "mvc";
            ocd.ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654";
            ocd.ConsentType = ConsentTypes.Explicit;
            ocd.DisplayName = "MVC client application";
            ocd.RedirectUris.Add(new Uri("https://localhost:44338/callback/login/local"));
            ocd.PostLogoutRedirectUris.Add(new Uri("https://localhost:44338/callback/logout/local"));

            ocd.Permissions.UnionWith(new[]
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Logout,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles});
            ocd.Requirements.Add(Requirements.Features.ProofKeyForCodeExchange);
        });
    }

    private Task CreateSamlClientIfNotExists(AsyncServiceScope scope)
    {
        return CreateClientIfNotExists(scope, "https://localhost:5001/saml", x =>
        {
            x.ClientId = "https://localhost:5001/saml";
            x.Permissions.UnionWith(new[] { Permissions.Scopes.Email });
        });
    }

    private async Task CreateClientIfNotExists(IServiceScope scope, string clientId, Action<OpenIddictApplicationDescriptor> descriptorCoonfiguration)
    {
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        if (await manager.FindByClientIdAsync(clientId) == null)
        {
            var newClientDescriptor = CreateAndConfigureOpenIddictApplicationDescriptor(descriptorCoonfiguration);
            await manager.CreateAsync(newClientDescriptor);
        }
    }

    private OpenIddictApplicationDescriptor CreateAndConfigureOpenIddictApplicationDescriptor(Action<OpenIddictApplicationDescriptor> configuration)
    {
        var od = new OpenIddictApplicationDescriptor();
        configuration(od);
        return od;
    }

    private Task CreateFakeUserBobIfNotExists(IServiceScope scope)
    {
        return CreateUserIfNotExists(scope, "bob@test.fake", user =>
        {
            user.UserName = "bob@test.fake";
            user.Email = "bob@test.fake";
        }, "Password123!");
    }

    private async Task CreateUserIfNotExists(IServiceScope scope, string userName, Action<ApplicationUser> userConfiguration, string userPassword)
    {
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        if (await userManager.FindByNameAsync(userName) == null)
        {
            var user = new ApplicationUser();
            userConfiguration(user);
            await userManager.CreateAsync(user, userPassword);
        }
    }

    private Task CreateEmailScopeIfNotExists(IServiceScope scope)
    {
        var claims = new[] { "email" };
        var serializedClaims = JsonSerializer.Serialize(claims);
        using var jsonDocument = JsonDocument.Parse(serializedClaims);
        var claimsElemennt = jsonDocument.RootElement.Clone();
        return CreateScopeIfNotExists(scope, "email", x =>
        {
            x.Name = "email";
            x.Resources.Add("https://localhost:5001/saml");
            x.Properties.Add("Claims", claimsElemennt);
        });
    }

    private static async Task CreateScopeIfNotExists(IServiceScope scope, string scopeName, Action<OpenIddictScopeDescriptor> scopeConfiguration)
    {
        var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
        if (await scopeManager.FindByNameAsync(scopeName) == null)
        {
            var scopeDescriptor = new OpenIddictScopeDescriptor();
            scopeConfiguration(scopeDescriptor);

            await scopeManager.CreateAsync(scopeDescriptor);
        }
    }

    private Task CreateSamlServiceProviderWithArtifactBindingIfNotExists(IServiceScope scope, string entityId, Action<ServiceProvider> serviceProviderConfiguration)
    {
        return CreateServiceProviderIfNotExists(scope, entityId, serviceProviderConfiguration);
    }

    private async Task CreateServiceProviderIfNotExists(IServiceScope scope, string entityId,
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

    private async Task SaveAllChanges(IServiceScope scope)
    {
        var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await applicationDbContext.SaveChangesAsync();
        var samlConfigurationDbContext = scope.ServiceProvider.GetRequiredService<ISamlConfigurationDbContext>();
        await samlConfigurationDbContext.SaveChangesAsync();
    }
}
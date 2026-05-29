using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rsk.Saml.Configuration;
using Rsk.Saml.IdentityProvider.Storage.EntityFramework.DbContexts;
using Rsk.Saml.IdentityProvider.Storage.EntityFramework;
using Rsk.Saml.IdentityProvider.Storage.EntityFramework.Mappers;
using Rsk.Saml.IdentityProvider.Storage.EntityFramework.Stores;
using Rsk.Saml.Samples;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using idpWithEf;
using ServiceProvider = Rsk.Saml.Models.ServiceProvider;

Console.Title = "IdentityServer4";

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.File("identityserver4_log.txt")
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate);
});

builder.Services.AddControllersWithViews();

// SAML SP database (DbContext)
builder.Services.AddDbContext<SamlConfigurationDbContext>(db => db.UseInMemoryDatabase("ServiceProviders"));
builder.Services.AddScoped<ISamlConfigurationDbContext, SamlConfigurationDbContext>();

IIdentityServerBuilder idsrvBuilder = builder.Services.AddIdentityServer(options =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
    })
    .AddTestUsers(TestUsers.Users)
    .AddInMemoryIdentityResources(Config.GetIdentityResources())
    .AddInMemoryApiResources(Config.GetApis())
    .AddInMemoryClients(Config.GetClients())
    .AddSigningCredential(new X509Certificate2("idsrv3test.pfx", "idsrv3test"));

// Configure SAML Identity Provider and authorized Service Providers
idsrvBuilder.AddSamlPlugin(options =>
    {
        options.Licensee = LicenseKey.Licensee;
        options.LicenseKey = LicenseKey.Key;
        options.WantAuthenticationRequestsSigned = false;
    })
    .AddServiceProviderStore<ServiceProviderStore>(); // Load authorized Service Providers from database

builder.Services.Configure<CookieAuthenticationOptions>(IdentityServerConstants.DefaultCookieAuthenticationScheme,
    cookie => { cookie.Cookie.Name = "idsrv.idp"; });

WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();

// Seed the ServiceProvider database
using (IServiceScope serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetService<SamlConfigurationDbContext>();
    if (!await context.ServiceProviders.AnyAsync())
    {
        foreach (ServiceProvider serviceProvider in Config.GetServiceProviders())
        {
            context.ServiceProviders.Add(serviceProvider.ToEntity());
        }
        await context.SaveChangesAsync();
    }
}

app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer()
   .UseIdentityServerSamlPlugin(); // enables SAML endpoints (e.g. ACS and SLO)

app.UseAuthorization();

app.MapDefaultControllerRoute();

await app.RunAsync();
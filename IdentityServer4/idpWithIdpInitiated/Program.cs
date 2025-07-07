using System;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.Saml.Configuration;
using Rsk.Saml.Samples;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using idpWithIdpInitiated;

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
    .AddInMemoryServiceProviders(Config.GetServiceProviders());

// use different cookie name that sp...
builder.Services.Configure<CookieAuthenticationOptions>(IdentityServerConstants.DefaultCookieAuthenticationScheme,
    cookie => { cookie.Cookie.Name = "idsrv.idpWithIdpInitiated"; });

WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer()
   .UseIdentityServerSamlPlugin(); // enables SAML endpoints (e.g. ACS and SLO)

app.UseAuthorization();

app.MapDefaultControllerRoute();

await app.RunAsync();
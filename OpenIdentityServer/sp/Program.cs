using System;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;
using Rsk.Saml.Configuration;
using Rsk.Saml.Samples;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using sp;

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
        .WriteTo.Console(
            outputTemplate:
            "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
            theme: AnsiConsoleTheme.Literate);
});

builder.WebHost.UseSetting("urls", "https://*:5001");

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
    .AddSigningCredential(new X509Certificate2("testclient.pfx", "test"));

// OPTIONAL - only required if you want to be a SAML IdP too
idsrvBuilder.AddSamlPlugin(options =>
    {
        options.Licensee = LicenseKey.Licensee;
        options.LicenseKey = LicenseKey.Key;
        options.WantAuthenticationRequestsSigned = false;
    })
    .AddInMemoryServiceProviders(Config.GetServiceProviders());

// SP configuration
builder.Services.AddAuthentication()
    .AddSaml2p("saml2p", options =>
    {
        options.Licensee = LicenseKey.Licensee;
        options.LicenseKey = LicenseKey.Key;

        // The IdP you want to integrate with
        options.IdentityProviderOptions = new IdpOptions
        {
            EntityId = "https://localhost:5000",
            SigningCertificates = { new X509Certificate2("idsrv3test.cer") },
            SingleSignOnEndpoint = new SamlEndpoint("https://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
            SingleLogoutEndpoint = new SamlEndpoint("https://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect),
        };

        // Details about yourself (the SP)
        options.ServiceProviderOptions = new SpOptions
        {
            EntityId = "https://localhost:5001/saml",
            MetadataPath = "/saml/metadata",
            SignAuthenticationRequests = true, // OPTIONAL - use if you want to sign your auth requests
            SigningCertificate = new X509Certificate2("testclient.pfx", "test")
        };

        options.NameIdClaimType = "sub";
        options.CallbackPath = "/signin-saml";
        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
    });

WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();

app.UseIdentityServer()
    .UseIdentityServerSamlPlugin(); // OPTIONAL - only required if you want to be a SAML IdP too

app.UseAuthorization();

app.MapDefaultControllerRoute();

await app.RunAsync();
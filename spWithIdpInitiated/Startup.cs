using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication()
    .AddCookie("cookie")
    .AddSaml2p("saml2p", options => {
        options.Licensee = "your DEMO Licensee";
        options.LicenseKey = "your DEMO LicenseKey";

        options.IdentityProviderOptions = new IdpOptions
        {
            EntityId = "https://localhost:5000",
            SigningCertificates = new List<X509Certificate2> { new X509Certificate2("idsrv3test.cer") },
            SingleSignOnEndpoint = new SamlEndpoint("https://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
            SingleLogoutEndpoint = new SamlEndpoint("https://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect),
        };

        options.ServiceProviderOptions = new SpOptions
        {
            EntityId = "https://localhost:5001/saml",
            MetadataPath = "/saml/metadata",
            SignAuthenticationRequests = false
        };

        options.NameIdClaimType = "sub";
        options.CallbackPath = "/signin-saml";
        options.SignInScheme = "cookie";

        // IdP-Initiated SSO
        options.AllowIdpInitiatedSso = true;
        options.IdPInitiatedSsoCompletionPath = "/";
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

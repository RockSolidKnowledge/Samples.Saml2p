using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;
using Rsk.Saml.Samples;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication()
    .AddCookie("cookie")
    .AddSaml2p("idp1", options => {
        options.Licensee = Constants.Licensee;
        options.LicenseKey = Constants.Licensee;

        options.IdentityProviderOptions = new IdpOptions
        {
            EntityId = "https://localhost:5000",
            SigningCertificates = { new X509Certificate2("idsrv3test.cer") },
            SingleSignOnEndpoint = new SamlEndpoint("https://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
            SingleLogoutEndpoint = new SamlEndpoint("https://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect),
        };

        options.ServiceProviderOptions = new SpOptions
        {
            EntityId = "https://localhost:5002/saml",
            MetadataPath = "/saml/metadata-saml-1",
            SignAuthenticationRequests = false
        };

        options.NameIdClaimType = "sub";
        options.CallbackPath = "/signin-saml-1";
        options.SignInScheme = "cookie";
    })
    .AddSaml2p("idp2", options => {
        options.Licensee = "/* your DEMO Licensee */";
        options.LicenseKey = "/* your DEMO LicenseKey */";

        options.IdentityProviderOptions = new IdpOptions
        {
            EntityId = "https://localhost:5001",
            SigningCertificates = { new X509Certificate2("testclient.cer") },
            SingleSignOnEndpoint = new SamlEndpoint("https://localhost:5001/saml/sso", SamlBindingTypes.HttpRedirect),
            SingleLogoutEndpoint = new SamlEndpoint("https://localhost:5001/saml/slo", SamlBindingTypes.HttpRedirect),
        };

        options.ServiceProviderOptions = new SpOptions
        {
            EntityId = "https://localhost:5002/saml",
            MetadataPath = "/saml/metadata-saml-2",
            SignAuthenticationRequests = false
        };

        options.NameIdClaimType = "sub";
        options.CallbackPath = "/signin-saml-2";
        options.SignInScheme = "cookie";
    })
    .AddSaml2p("duende", options =>
    {
        options.Licensee = "/* your DEMO Licensee */";
        options.LicenseKey = "/* your DEMO LicenseKey */";

        options.IdentityProviderMetadataAddress = "https://localhost:5003/saml/metadata";

        options.ServiceProviderOptions = new SpOptions
        {
            EntityId = "https://localhost:5002/saml",
            MetadataPath = "/saml/metadata-saml-3",
            SignAuthenticationRequests = false
        };

        options.NameIdClaimType = "sub";
        options.CallbackPath = "/signin-saml-3";
        options.SignInScheme = "cookie";
    });

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

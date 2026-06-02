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
    .AddSaml2p("saml-1", options => {
        options.Licensee = "<insert here>";
        options.LicenseKey = "<insert here>";

        // SAML Metadata address to automatically populate options
        options.IdentityProviderMetadataAddress = "<insert here>";

        // Uncomment to manually provide IdentityProvider options
        //options.IdentityProviderOptions = new IdpOptions();

        // Details about this service provider
        options.ServiceProviderOptions = new SpOptions
        {
          EntityId = "https://localhost:5001/saml",
          MetadataPath = "/saml-1/metadata",
          SignAuthenticationRequests = false
        };
        

        options.NameIdClaimType = "sub";

        //Unique Callback path for this scheme
        options.CallbackPath = "/signin-saml-1";
        options.SignInScheme = "cookie";

        // Allow IdP-Initiated SSO
        options.AllowIdpInitiatedSso = true;
        // Where to redirect the IDP SSO User post Sign-in e.g for account provisioning 
        options.IdPInitiatedSsoCompletionPath = "/";
    })
    .AddSaml2p("saml-2", options => {
        options.Licensee = "<insert here>";
        options.LicenseKey = "<insert here>";

        // SAML Metadata address to automatically populate options
        options.IdentityProviderMetadataAddress = "<insert here>";
        
        // Uncomment to manually provide IdentityProvider options
        //options.IdentityProviderOptions = new IdpOptions();
        
        // Details about this service provider
        options.ServiceProviderOptions = new SpOptions
        {
            EntityId = "https://localhost:5001/saml",
            MetadataPath = "/saml-2/metadata",
            SignAuthenticationRequests = false
        };

        options.NameIdClaimType = "sub";
        
        //Unique Callback path for this scheme
        options.CallbackPath = "/signin-saml-2";
        options.SignInScheme = "cookie";

        // Allow IdP-Initiated SSO
        options.AllowIdpInitiatedSso = true;
        // Where to redirect the IDP SSO User post Sign-in e.g for account provisioning 
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

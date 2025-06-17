using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Rsk.Saml.Samples;
using Rsk.Saml.Samples.Services;
using Rsk.Saml.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication()
    .AddCookie("cookie")
    .AddSaml2p("saml2p", options => {
        options.Licensee = LicenseKey.Licensee;
        options.LicenseKey = LicenseKey.Key;
        
        options.IdentityProviderMetadataAddress = "https://localhost:5003/saml/metadata";

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
        
        options.Events = new RemoteAuthenticationEvents()
        {
            OnTicketReceived = context =>
            {
                context.Properties.Items.TryGetValue("RelayState", out var relayStateValue);
                // do something with relayStateValue

                return Task.FromResult(0);
            }
        };
    });

builder.Services.AddScoped<ISamlRelayStateValidator, CustomSamlRelayStateValidator>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

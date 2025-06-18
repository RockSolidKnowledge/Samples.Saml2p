using System.Security.Cryptography.X509Certificates;
using Duende.IdentityServer;
using DuendeDynamicProviders.Services;
using Rsk.AspNetCore.Authentication.Saml2p;
using Rsk.Saml.DuendeIdentityServer.DynamicProviders;
using Rsk.Saml.Samples;
using Rsk.Saml.Services;
using Serilog;

namespace DuendeDynamicProviders;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddRazorPages();

        var isBuilder = builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            .AddTestUsers(TestUsers.Users);

        // in-memory, code config
        isBuilder.AddInMemoryIdentityResources(Config.IdentityResources);
        isBuilder.AddInMemoryApiScopes(Config.ApiScopes);
        isBuilder.AddInMemoryClients(Config.Clients);

        // SP configuration - dynamic providers
        isBuilder.AddSamlDynamicProvider(options =>
        {
            // unstorable/reusable data, such as license information and events. This will override the data stored
            options.Licensee = LicenseKey.Licensee;
            options.LicenseKey = LicenseKey.Key;
        })

            // Use EntityFramework store for storing identity providers
            //.AddIdentityProviderStore<SamlIdentityProviderStore>();

            // use in memory store for storing identity providers
            .AddInMemoryIdentityProviders(new List<SamlDynamicIdentityProvider>
            {
                    new SamlDynamicIdentityProvider
                    {
                        SamlAuthenticationOptions = new Saml2pAuthenticationOptions
                        {
                            IdentityProviderMetadataAddress = "https://localhost:5003/saml/metadata",

                            // Details about yourself (the SP)
                            ServiceProviderOptions = new SpOptions
                            {
                                EntityId = "https://localhost:5004/saml",
                                MetadataPath = "/federation/saml/metadata",
                                SignAuthenticationRequests = false // OPTIONAL - use if you want to sign your auth requests
                            },

                            NameIdClaimType = "sub",
                            CallbackPath = "/federation/saml/signin-saml", // Duende prefixes "/federation/{scheme}" to all paths
                            SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                            
                            AllowIdpInitiatedSso = true,
                            
                            IdPInitiatedSsoCompletionPath = "/IdpInitiatedCompleted/RedirectToRelayState"
                        },

                        Scheme = "saml",
                        DisplayName = "saml",
                        Enabled = true,
                    }
            });

        builder.Services.AddAuthentication();
        
        builder.Services.AddScoped<ISamlRelayStateValidator, CustomSamlRelayStateValidator>();

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        app.UseDeveloperExceptionPage();

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        
        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}
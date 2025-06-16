using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Rsk.Saml.Configuration;
using Rsk.Saml.Samples;
using Serilog;

namespace DuendeIdP;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddRazorPages();

        var isBuilder = builder.Services.AddIdentityServer(options =>
            {
                options.KeyManagement.Enabled = true;
                options.KeyManagement.SigningAlgorithms = new[] {
                    new SigningAlgorithmOptions("RS256") {UseX509Certificate = true}
                };

                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            .AddTestUsers(TestUsers.Users);

        // in-memory, code config
        isBuilder.AddInMemoryIdentityResources(Config.GetIdentityResources());
        isBuilder.AddInMemoryApiScopes(Config.GetApiScopes());
        isBuilder.AddInMemoryClients(Config.GetClients());

        isBuilder.AddSamlPlugin(options =>
            {
                options.Licensee = LicenseKey.Licensee;
                options.LicenseKey = LicenseKey.Key;

                options.WantAuthenticationRequestsSigned = false;
            })
            .AddInMemoryServiceProviders(Config.GetServiceProviders());

        // use different cookie name that sp...
        builder.Services.Configure<CookieAuthenticationOptions>(IdentityServerConstants.DefaultCookieAuthenticationScheme,
            cookie => { cookie.Cookie.Name = "idsrv.idp"; });

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        app.UseDeveloperExceptionPage();

        app.UseStaticFiles();
        app.UseRouting();

        app.UseIdentityServer()
            .UseIdentityServerSamlPlugin(); // enables SAML endpoints (e.g. ACS and SLO)

        app.UseAuthorization();
        
        app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        
        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}
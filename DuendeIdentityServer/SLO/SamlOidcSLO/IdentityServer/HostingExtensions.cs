using Duende.IdentityServer.Configuration;
using IdentityServerHost;
using Rsk.Saml.Configuration;
using Rsk.Saml.Samples;
using Serilog;

namespace IdentityServer;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        builder.Services.AddIdentityServer(options =>
            {
                options.KeyManagement.Enabled = true;

                //Required to use the SAML plugin with Key Management
                options.KeyManagement.SigningAlgorithms = new[]
                {
                    new SigningAlgorithmOptions("RS256")
                    {
                        UseX509Certificate = true
                    }
                };
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddTestUsers(TestUsers.Users)
            .AddSamlPlugin(options =>
            {
                options.Licensee = LicenseKey.Licensee;
                options.LicenseKey = LicenseKey.Key;

                //Use Iterative SLO
                options.UseIFramesForSlo = false;
            })
            .AddInMemoryServiceProviders(Config.ServiceProvider)
            .AddInMemoryPersistedGrants();

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
            
        app.UseIdentityServer()
            .UseIdentityServerSamlPlugin();
        
        app.UseAuthorization();
        app.MapRazorPages().RequireAuthorization();

        return app;
    }
}
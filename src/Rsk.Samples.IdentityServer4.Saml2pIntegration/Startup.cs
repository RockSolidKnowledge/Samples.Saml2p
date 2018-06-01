using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Rsk.Samples.IdentityServer4.Saml2pIntegration
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentityServer()
                .AddSigningCredential(new X509Certificate2("idsrv3test.pfx", "idsrv3test"))
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(TestUsers.Users)
                .AddSamlPlugin(options =>
                {
                    options.WantAuthenticationRequestsSigned = false;

                    options.Licensee = "";
                    options.LicenseKey = "";
                })
                .AddInMemoryServiceProviders(Config.GetServiceProviders());
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            app.UseDeveloperExceptionPage();

            app.UseIdentityServer()
                .UseIdentityServerSamlPlugin();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
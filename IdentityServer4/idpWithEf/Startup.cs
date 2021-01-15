using System.Linq;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Rsk.Saml.Configuration;
using Rsk.Saml.IdentityProvider.Storage.EntityFramework.DbContexts;
using Rsk.Saml.IdentityProvider.Storage.EntityFramework.Interfaces;
using Rsk.Saml.IdentityProvider.Storage.EntityFramework.Mappers;
using Rsk.Saml.IdentityProvider.Storage.EntityFramework.Stores;

namespace idpWithEf
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // SAML SP database (DbContext)
            services.AddDbContext<SamlConfigurationDbContext>(db => 
                db.UseInMemoryDatabase("ServiceProviders"));
            services.AddScoped<ISamlConfigurationDbContext, SamlConfigurationDbContext>();

            var builder = services.AddIdentityServer(options =>
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
            builder.AddSamlPlugin(options =>
                {
                    options.Licensee = "";
                    options.LicenseKey = "";

                    options.WantAuthenticationRequestsSigned = false;
                })
                .AddServiceProviderStore<ServiceProviderStore>(); // Load authorized Service Providers from database
            
            builder.Services.Configure<CookieAuthenticationOptions>(IdentityServerConstants.DefaultCookieAuthenticationScheme,
                cookie => { cookie.Cookie.Name = "idsrv.idp"; });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseDeveloperExceptionPage();

            SeedServiceProviderDatabase(app);

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer()
               .UseIdentityServerSamlPlugin(); // enables SAML endpoints (e.g. ACS and SLO)

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }

        private void SeedServiceProviderDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<SamlConfigurationDbContext>();
                if (!context.ServiceProviders.Any())
                {
                    foreach (var serviceProvider in Config.GetServiceProviders())
                    {
                        context.ServiceProviders.Add(serviceProvider.ToEntity());
                    }

                    context.SaveChanges();
                }
            }
        }
    }
}
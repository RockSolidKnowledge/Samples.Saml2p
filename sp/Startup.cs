using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;

namespace sp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddControllersWithViews();

            services.Configure<IISOptions>(options =>
            {
                options.AutomaticAuthentication = false;
                options.AuthenticationDisplayName = "Windows";
            });

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddTestUsers(TestUsers.Users)
                .AddSigningCredential(new X509Certificate2("testclient.pfx", "test"));

            builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
            builder.AddInMemoryApiResources(Config.GetApis());
            builder.AddInMemoryClients(Config.GetClients())
                .AddSamlPlugin(options =>
                {
                    options.Licensee = "";
                    options.LicenseKey = "";
                    options.WantAuthenticationRequestsSigned = false;
                })
                .AddInMemoryServiceProviders(Config.GetServiceProviders());

            services.AddAuthentication()
                .AddSaml2p("saml2p", options => {
                    options.Licensee = "";
                    options.LicenseKey = "";

                    options.IdentityProviderOptions = new IdpOptions
                    {
                        EntityId = "https://localhost:5000",
                        SigningCertificate = new X509Certificate2("idsrv3test.cer"),
                        SingleSignOnEndpoint = new SamlEndpoint("https://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
                        SingleLogoutEndpoint = new SamlEndpoint("https://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect),
                    };

                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "https://localhost:5001/saml",
                        MetadataPath = "/saml/metadata",
                        SignAuthenticationRequests = true,
                        SigningCertificate = new X509Certificate2("testclient.pfx", "test")
                    };

                    options.NameIdClaimType = "sub";
                    options.CallbackPath = "/signin-saml";
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer()
               .UseIdentityServerSamlPlugin();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
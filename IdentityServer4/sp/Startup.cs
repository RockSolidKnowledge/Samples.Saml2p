using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;
using Rsk.Saml.Configuration;

namespace sp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

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
                .AddSigningCredential(new X509Certificate2("testclient.pfx", "test"));

            // OPTIONAL - only required if you want to be a SAML IdP too
            builder.AddSamlPlugin(options =>
                {
                    options.Licensee = "your DEMO Licensee";
                    options.LicenseKey = "your DEMO LicenseKey";

                    options.WantAuthenticationRequestsSigned = false;
                })
                .AddInMemoryServiceProviders(Config.GetServiceProviders());

            // SP configuration
            services.AddAuthentication()
                .AddSaml2p("saml2p", options => {
                    options.Licensee = "";
                    options.LicenseKey = "";

                    // The IdP you want to integrate with
                    options.IdentityProviderOptions = new IdpOptions
                    {
                        EntityId = "https://localhost:5000",
                        SigningCertificates = {new X509Certificate2("idsrv3test.cer")},
                        SingleSignOnEndpoint = new SamlEndpoint("https://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
                        SingleLogoutEndpoint = new SamlEndpoint("https://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect),
                    };

                    // Details about yourself (the SP)
                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "https://localhost:5001/saml",
                        MetadataPath = "/saml/metadata",
                        SignAuthenticationRequests = true, // OPTIONAL - use if you want to sign your auth requests
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
                .UseIdentityServerSamlPlugin(); // OPTIONAL - only required if you want to be a SAML IdP too

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
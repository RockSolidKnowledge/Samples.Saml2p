using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;

namespace spWithIdpInitiated
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthentication()
                .AddCookie("cookie")
                .AddSaml2p("saml2p", options => {
                    options.Licensee = "";
                    options.LicenseKey = "";

                    options.IdentityProviderOptions = new IdpOptions
                    {
                        EntityId = "http://localhost:5000",
                        SigningCertificates = new List<X509Certificate2> { new X509Certificate2("idsrv3test.cer") },
                        SingleSignOnEndpoint = new SamlEndpoint("http://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
                        SingleLogoutEndpoint = new SamlEndpoint("http://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect),
                    };

                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "http://localhost:5001/saml",
                        MetadataPath = "/saml/metadata",
                        SignAuthenticationRequests = false
                    };

                    options.NameIdClaimType = "sub";
                    options.CallbackPath = "/signin-saml";
                    options.SignInScheme = "cookie";

                    // IdP-Initiated SSO
                    options.AllowIdpInitiatedSso = true;
                    options.IdPInitiatedSsoCompletionPath = "/";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;

namespace spWithMultipleIdps
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication()
                .AddCookie("cookie")
                .AddSaml2p("idp1", options => {
                    options.Licensee = "";
                    options.LicenseKey = "";

                    options.IdentityProviderOptions = new IdpOptions
                    {
                        EntityId = "http://localhost:5000",
                        SigningCertificate = new X509Certificate2("idsrv3test.cer"),
                        SingleSignOnEndpoint = new SamlEndpoint("http://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
                        SingleLogoutEndpoint = new SamlEndpoint("http://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect),
                    };

                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "http://localhost:5002/saml",
                        MetadataPath = "/saml/metadata",
                        SignAuthenticationRequests = false
                    };

                    options.NameIdClaimType = "sub";
                    options.CallbackPath = "/signin-saml-1";
                    options.SignInScheme = "cookie";
                })
                .AddSaml2p("idp2", options => {
                    options.Licensee = "";
                    options.LicenseKey = "";

                    options.IdentityProviderOptions = new IdpOptions
                    {
                        EntityId = "http://localhost:5001",
                        SigningCertificate = new X509Certificate2("testclient.cer"),
                        SingleSignOnEndpoint = new SamlEndpoint("http://localhost:5001/saml/sso", SamlBindingTypes.HttpRedirect),
                        SingleLogoutEndpoint = new SamlEndpoint("http://localhost:5001/saml/slo", SamlBindingTypes.HttpRedirect),
                    };

                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "http://localhost:5002/saml",
                        MetadataPath = "/saml/metadata",
                        SignAuthenticationRequests = false
                    };

                    options.NameIdClaimType = "sub";
                    options.CallbackPath = "/signin-saml-2";
                    options.SignInScheme = "cookie";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
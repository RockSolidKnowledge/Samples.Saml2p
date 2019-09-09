using System;
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
                                                                                                    
            X509Certificate2 signatureCertWithPrivate = new X509Certificate2(Convert.FromBase64String("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"));
            X509Certificate2 encryptionCertWithPrivate = new X509Certificate2(Convert.FromBase64String("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"));

            services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            });

            services.AddAuthentication()
                .AddCookie("cookie")
                .AddSaml2p("pkcs1", options => {
                    options.Licensee = "ENTER_VALIDLICENSEE";
                    options.LicenseKey = "ENTER_VALIDLICENSEE";

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
                        SignAuthenticationRequests = true,
                        SigningCertificate = signatureCertWithPrivate,
                        EncryptionCertificate = encryptionCertWithPrivate
                    };

                    options.NameIdClaimType = "sub";
                    options.CallbackPath = "/signin-saml-pkcs1";
                    options.SignInScheme = "cookie";
                })
                .AddSaml2p("oaep", options => {
                    options.Licensee = "ENTER_VALIDLICENSEE";
                    options.LicenseKey = "ENTER_VALIDLICENSEE";

                    options.IdentityProviderOptions = new IdpOptions
                    {
                        EntityId = "http://localhost:5000",
                        SigningCertificate = new X509Certificate2("idsrv3test.cer"),
                        SingleSignOnEndpoint = new SamlEndpoint("http://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
                        SingleLogoutEndpoint = new SamlEndpoint("http://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect),
                    };


                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "http://localhost:5002/saml-oaep",
                        MetadataPath = "/saml/metadata",
                        SignAuthenticationRequests = true,
                        SigningCertificate = signatureCertWithPrivate,
                        EncryptionCertificate = encryptionCertWithPrivate
                    };

                    options.NameIdClaimType = "sub";
                    options.CallbackPath = "/signin-saml-oaep";
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
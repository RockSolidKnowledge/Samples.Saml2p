using System;
using System.IdentityModel.Metadata;
using System.Security.Cryptography.X509Certificates;
using System.Web.Hosting;
using Kentor.AuthServices;
using Kentor.AuthServices.Configuration;
using Kentor.AuthServices.Owin;
using Kentor.AuthServices.WebSso;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(TestClient.Kentor.Startup))]

namespace TestClient.Kentor
{
    public class Startup
    {
        // Using Kentor.AuthServices
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions { AuthenticationType = "cookie" });

            var spOptions = new SPOptions
            {
                EntityId = new EntityId("http://localhost:50155/AuthServices"),
                AuthenticateRequestSigningBehavior = SigningBehavior.Always
            };
            spOptions.ServiceCertificates.Add(new ServiceCertificate
            {
                Certificate = new X509Certificate2(HostingEnvironment.MapPath("~/testclient.pfx"), "test"),
                Use = CertificateUse.Signing
            });

            var options = new KentorAuthServicesAuthenticationOptions(false)
            {
                SPOptions = spOptions,
                AuthenticationType = "saml2p",
                SignInAsAuthenticationType = "cookie"
            };

            var idp = new IdentityProvider(new EntityId("http://localhost:5000"), options.SPOptions)
            {
                SingleSignOnServiceUrl = new Uri("http://localhost:5000/saml/sso"),
                Binding = Saml2BindingType.HttpRedirect,
                SingleLogoutServiceUrl = new Uri("http://localhost:5000/saml/slo"),
                SingleLogoutServiceBinding = Saml2BindingType.HttpPost,
                WantAuthnRequestsSigned = true
            };
            idp.SigningKeys.AddConfiguredKey(new X509Certificate2(HostingEnvironment.MapPath("~/idsrv3test.cer")));

            options.IdentityProviders.Add(idp);

            app.UseKentorAuthServicesAuthentication(options);
        }
    }
}

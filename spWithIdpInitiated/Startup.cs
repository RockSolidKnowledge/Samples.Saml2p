using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Rsk.AspNetCore.Authentication.Saml2p;

namespace sp
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

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
                    options.Licensee = "Demo";
                    options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsImF1dGgiOiJERU1PIiwiZXhwIjoiMjAxOS0wOS0wOVQwMDowMDowMCIsImlhdCI6IjIwMTktMDgtMDlUMDg6MDY6NDEuMDAwNjU4MSIsIm9yZyI6IkRFTU8iLCJhdWQiOjJ9.FzcifjW8YUPu6icRjrMn1NYv/KJt7ks/m/TrUTZJO1yPDns9v1nNM4++jHObmazmmpzepBQrxJYZpIwEMCovVAubEptH9jIAEr+Y6/tgMQurql9UocrfgPr15J5520XqLB7ExOwCsGI2bm3f0pY/bx2H5Pesg92Ii11S/ELHvnQ3JdyTnaLqpMbjXA0Wcc2jZS/VZ2cpNTItXzA4SxYMaSF1qh3N7ZlkTk+zv8RKHYimdKgKr+bVEc9SS/VEr2NpJcq/dyDl/bwhRg2N9by8h+SdZhyX0ELA50RB8vIUqwG281Upye4cfmY0jlsAyrcU2IvEbokq3Q/IpnuVIFh1Q7Ymh0dP1T327e5n+YvqvZQCZdF2nnAmwQ3P85Kg9TFxkXbbAEEDQ+5yTyllOqsRVVxZ8JCuyQY1Rub6DGN+gKoC/BLQ0+HbdPxcXLr3OX/wj/7/XuQzoLU8bOLC/oNj2tITI10U3W2ycx/F38+nosdLeTVMBfKuJ+rGvFbRYmpuXdBRwxClgYEfOa5gq9aGBvmE2fFHqwkSOcWK2tVLwcN0OqOkkX5D7sn2F5RbXhhentFvRGqAAP8BKEA1214SWzLrAdCxVLOcOk72B8HcRNPPJjnH32CeGwH/w+vGsatvlrz1zpRZdqcjKjrFAxIO4YDvNQ67unZKAR+o7i8CI+s=";

                    options.IdentityProviderOptions = new IdpOptions
                    {
                        EntityId = "http://localhost:5000",
                        SigningCertificate = new X509Certificate2("idsrv3test.cer"),
                        SingleSignOnEndpoint = new SamlEndpoint("http://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
                        SingleLogoutEndpoint = new SamlEndpoint("http://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect),
                    };

                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "http://localhost:5001/saml",
                        MetadataPath = "/saml/metadata",
                        SignAuthenticationRequests = true,
                        SigningCertificate = new X509Certificate2("testclient.pfx", "test")
                    };

                    options.NameIdClaimType = "sub";
                    options.CallbackPath = "/signin-saml";
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    // IdP-Initiated SSO
                    options.AllowIdpInitiatedSso = true;
                    options.IdPInitiatedSsoCompletionPath = "/External/IdpInitiatedCallback";

                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseIdentityServer()
               .UseIdentityServerSamlPlugin();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
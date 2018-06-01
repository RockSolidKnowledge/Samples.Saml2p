using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;

namespace TestClient.Rsk
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = "saml";
                })
                .AddCookie()
                .AddSaml2p("saml", options =>
                {
                    options.Licensee = "";
                    options.LicenseKey = "";
                    options.IdentityProviderOptions = new IdpOptions
                    {
                        EntityId = "http://localhost:5000",
                        BindingType = SamlBindingTypes.HttpRedirect,
                        SsoEndpoint = "http://localhost:5000/saml/sso",
                        SigningCertificate = new X509Certificate2("idsrv3test.cer")
                    };

                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "http://localhost:5001/saml",
                        SignAuthenticationRequests = false,
                        MetadataPath = "/saml",
                        SigningCertificate = new X509Certificate2("testclient.pfx", "test")
                    };
                    
                    options.CallbackPath = "/signin-saml";
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}

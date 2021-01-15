// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Duende.IdentityServer;
using Duende.IdentityServer.Configuration;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rsk.Saml.Configuration;

namespace duendetest
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var builder = services.AddIdentityServer(options =>
            {
                options.KeyManagement.Enabled = true;
                options.KeyManagement.SigningAlgorithms = new [] {
                    new SigningAlgorithmOptions("RS256") {UseX509Certificate = true}
                };

                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v5/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
                .AddTestUsers(TestUsers.Users);

            // in-memory, code config
            builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
            builder.AddInMemoryApiScopes(Config.GetApiScopes());
            //builder.AddInMemoryApiResources(Config.GetApis());
            builder.AddInMemoryClients(Config.GetClients());

            builder.AddSamlPlugin(options =>
                {
                    options.Licensee = "DEMO";
                    options.LicenseKey =
                        "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjEtMDUtMDdUMDA6MDA6MDAiLCJpYXQiOiIyMDIxLTAxLTA3VDE0OjI2OjQ4Iiwib3JnIjoiREVNTyIsImF1ZCI6Mn0=.TrqnyKp2aJ2p/XT3FiXiM1yI57TfK8rudTU0ns9LPU+OI85Aqflm5L1DEqGgXVqBlciPdAFQjUo5EfDFXHsA1TDwsu6DH73LOYzrzuL6YJ7ueWgSmT+pRSbOF0/5ykeiJn8XkdhpIiGDl+g5rxfEnhwbM4pYALlDPbWr4Oh/EmIIwM58fKEsPyAGGN3TkZ4Hcuic3lh8mS0DxWPtu2TNUUjAybLkQL/Iy+EvGsM07zWtiAiJ81SWJeoQpwCpKgq75ULhV1D+58yFc7NwGpMBA+GULQPlJpS8oBgpwZk2cO5f5p0MUoW3FI6wa1flA+AGx/38kzjePyqxaY56EAuVihOnc7HOdFgDyi2e4HmjqbTlQSFQNkllMm3fo2KGo7B5VxpgkYEyEGC2/RnhCn9rAymmd1HGISKkqm+dk2wTkjJAd8zGQLyQQM0FpajSOhKjrhi6MTGJmq9EhEnwuFISy58kcyTTq+0R72OByJKPGRNq42D3bMOVZrTgygI1aQ718sEAlj+9v6jBNy9uLwEPs69Yox4TPaLwQ8rM1HThOs6MhDgEbiPhIy1NJeUJGp46a2+a4357tTtG7VRO+vWxwzulV2YhSVglU+ORF/oA4zfPla3S9wUBb8DW5ZmhY2a0LsjRF30NZp1o+g0b7tkhZyIT8LiExG5DnMnK6gRiwyM=";

                    options.WantAuthenticationRequestsSigned = false;
                })
                .AddInMemoryServiceProviders(Config.GetServiceProviders());

            // use different cookie name that sp...
            builder.Services.Configure<CookieAuthenticationOptions>(IdentityServerConstants.DefaultCookieAuthenticationScheme,
                cookie => { cookie.Cookie.Name = "idsrv.idp"; });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer()
                .UseIdentityServerSamlPlugin(); // enables SAML endpoints (e.g. ACS and SLO)

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
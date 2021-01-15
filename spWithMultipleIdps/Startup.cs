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
            services.AddControllersWithViews();

            services.AddAuthentication()
                .AddCookie("cookie")
                .AddSaml2p("idp1", options => {
                    options.Licensee = "DEMO";
                    options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjEtMDUtMDdUMDA6MDA6MDAiLCJpYXQiOiIyMDIxLTAxLTA3VDE0OjI2OjQ4Iiwib3JnIjoiREVNTyIsImF1ZCI6Mn0=.TrqnyKp2aJ2p/XT3FiXiM1yI57TfK8rudTU0ns9LPU+OI85Aqflm5L1DEqGgXVqBlciPdAFQjUo5EfDFXHsA1TDwsu6DH73LOYzrzuL6YJ7ueWgSmT+pRSbOF0/5ykeiJn8XkdhpIiGDl+g5rxfEnhwbM4pYALlDPbWr4Oh/EmIIwM58fKEsPyAGGN3TkZ4Hcuic3lh8mS0DxWPtu2TNUUjAybLkQL/Iy+EvGsM07zWtiAiJ81SWJeoQpwCpKgq75ULhV1D+58yFc7NwGpMBA+GULQPlJpS8oBgpwZk2cO5f5p0MUoW3FI6wa1flA+AGx/38kzjePyqxaY56EAuVihOnc7HOdFgDyi2e4HmjqbTlQSFQNkllMm3fo2KGo7B5VxpgkYEyEGC2/RnhCn9rAymmd1HGISKkqm+dk2wTkjJAd8zGQLyQQM0FpajSOhKjrhi6MTGJmq9EhEnwuFISy58kcyTTq+0R72OByJKPGRNq42D3bMOVZrTgygI1aQ718sEAlj+9v6jBNy9uLwEPs69Yox4TPaLwQ8rM1HThOs6MhDgEbiPhIy1NJeUJGp46a2+a4357tTtG7VRO+vWxwzulV2YhSVglU+ORF/oA4zfPla3S9wUBb8DW5ZmhY2a0LsjRF30NZp1o+g0b7tkhZyIT8LiExG5DnMnK6gRiwyM=";

                    options.IdentityProviderOptions = new IdpOptions
                    {
                        EntityId = "https://localhost:5000",
                        SigningCertificates = { new X509Certificate2("idsrv3test.cer") },
                        SingleSignOnEndpoint = new SamlEndpoint("https://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
                        SingleLogoutEndpoint = new SamlEndpoint("https://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect),
                    };

                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "https://localhost:5002/saml",
                        MetadataPath = "/saml/metadata",
                        SignAuthenticationRequests = false
                    };

                    options.NameIdClaimType = "sub";
                    options.CallbackPath = "/signin-saml-1";
                    options.SignInScheme = "cookie";
                })
                .AddSaml2p("idp2", options => {
                    options.Licensee = "DEMO";
                    options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjEtMDUtMDdUMDA6MDA6MDAiLCJpYXQiOiIyMDIxLTAxLTA3VDE0OjI2OjQ4Iiwib3JnIjoiREVNTyIsImF1ZCI6Mn0=.TrqnyKp2aJ2p/XT3FiXiM1yI57TfK8rudTU0ns9LPU+OI85Aqflm5L1DEqGgXVqBlciPdAFQjUo5EfDFXHsA1TDwsu6DH73LOYzrzuL6YJ7ueWgSmT+pRSbOF0/5ykeiJn8XkdhpIiGDl+g5rxfEnhwbM4pYALlDPbWr4Oh/EmIIwM58fKEsPyAGGN3TkZ4Hcuic3lh8mS0DxWPtu2TNUUjAybLkQL/Iy+EvGsM07zWtiAiJ81SWJeoQpwCpKgq75ULhV1D+58yFc7NwGpMBA+GULQPlJpS8oBgpwZk2cO5f5p0MUoW3FI6wa1flA+AGx/38kzjePyqxaY56EAuVihOnc7HOdFgDyi2e4HmjqbTlQSFQNkllMm3fo2KGo7B5VxpgkYEyEGC2/RnhCn9rAymmd1HGISKkqm+dk2wTkjJAd8zGQLyQQM0FpajSOhKjrhi6MTGJmq9EhEnwuFISy58kcyTTq+0R72OByJKPGRNq42D3bMOVZrTgygI1aQ718sEAlj+9v6jBNy9uLwEPs69Yox4TPaLwQ8rM1HThOs6MhDgEbiPhIy1NJeUJGp46a2+a4357tTtG7VRO+vWxwzulV2YhSVglU+ORF/oA4zfPla3S9wUBb8DW5ZmhY2a0LsjRF30NZp1o+g0b7tkhZyIT8LiExG5DnMnK6gRiwyM=";


                    options.IdentityProviderOptions = new IdpOptions
                    {
                        EntityId = "https://localhost:5001",
                        SigningCertificates = {new X509Certificate2("testclient.cer")},
                        SingleSignOnEndpoint = new SamlEndpoint("https://localhost:5001/saml/sso", SamlBindingTypes.HttpRedirect),
                        SingleLogoutEndpoint = new SamlEndpoint("https://localhost:5001/saml/slo", SamlBindingTypes.HttpRedirect),
                    };

                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "https://localhost:5002/saml",
                        MetadataPath = "/saml/metadata",
                        SignAuthenticationRequests = false
                    };

                    options.NameIdClaimType = "sub";
                    options.CallbackPath = "/signin-saml-2";
                    options.SignInScheme = "cookie";
                })
                .AddSaml2p("duende", options =>
                {
                    options.Licensee = "DEMO";
                    options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjEtMDUtMDdUMDA6MDA6MDAiLCJpYXQiOiIyMDIxLTAxLTA3VDE0OjI2OjQ4Iiwib3JnIjoiREVNTyIsImF1ZCI6Mn0=.TrqnyKp2aJ2p/XT3FiXiM1yI57TfK8rudTU0ns9LPU+OI85Aqflm5L1DEqGgXVqBlciPdAFQjUo5EfDFXHsA1TDwsu6DH73LOYzrzuL6YJ7ueWgSmT+pRSbOF0/5ykeiJn8XkdhpIiGDl+g5rxfEnhwbM4pYALlDPbWr4Oh/EmIIwM58fKEsPyAGGN3TkZ4Hcuic3lh8mS0DxWPtu2TNUUjAybLkQL/Iy+EvGsM07zWtiAiJ81SWJeoQpwCpKgq75ULhV1D+58yFc7NwGpMBA+GULQPlJpS8oBgpwZk2cO5f5p0MUoW3FI6wa1flA+AGx/38kzjePyqxaY56EAuVihOnc7HOdFgDyi2e4HmjqbTlQSFQNkllMm3fo2KGo7B5VxpgkYEyEGC2/RnhCn9rAymmd1HGISKkqm+dk2wTkjJAd8zGQLyQQM0FpajSOhKjrhi6MTGJmq9EhEnwuFISy58kcyTTq+0R72OByJKPGRNq42D3bMOVZrTgygI1aQ718sEAlj+9v6jBNy9uLwEPs69Yox4TPaLwQ8rM1HThOs6MhDgEbiPhIy1NJeUJGp46a2+a4357tTtG7VRO+vWxwzulV2YhSVglU+ORF/oA4zfPla3S9wUBb8DW5ZmhY2a0LsjRF30NZp1o+g0b7tkhZyIT8LiExG5DnMnK6gRiwyM=";

                    options.IdentityProviderMetadataAddress = "https://localhost:5003/saml/metadata";

                    options.ServiceProviderOptions = new SpOptions
                    {
                        EntityId = "https://localhost:5002/saml",
                        MetadataPath = "/saml/metadata",
                        SignAuthenticationRequests = false
                    };

                    options.NameIdClaimType = "sub";
                    options.CallbackPath = "/signin-saml-3";
                    options.SignInScheme = "cookie";
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
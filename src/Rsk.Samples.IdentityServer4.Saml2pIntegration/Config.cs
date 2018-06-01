using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Saml;
using IdentityServer4.Saml.Models;

namespace Rsk.Samples.IdentityServer4.Saml2pIntegration
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResources.Phone()
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new []
            {
                new ApiResource("api1", "My API #1")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client
                {
                    ClientId = "http://localhost:50155/AuthServices",
                    ClientName = "Kentor SAML2P Test Client",
                    ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                    AllowedScopes = { "openid", "profile" }
                },
                new Client
                {
                    ClientId = "http://localhost:5001/saml",
                    ClientName = "RSK SAML2P Test Client",
                    ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                    AllowedScopes = { "openid", "profile" }
                }
            };
        }

        public static IEnumerable<ServiceProvider> GetServiceProviders()
        {
            return new[]
            {
                new ServiceProvider
                {
                    EntityId = "http://localhost:50155/AuthServices",
                    SigningCertificates = {new X509Certificate2("TestClient.cer")},
                    AssertionConsumerServices =
                    {
                        new Service(SamlConstants.BindingTypes.HttpPost, "http://localhost:50155/AuthServices/Acs", 1)
                    }
                },
                new ServiceProvider
                {
                    EntityId = "http://localhost:5001/saml",
                    SigningCertificates = {new X509Certificate2("TestClient.cer")},
                    AssertionConsumerServices =
                    {
                        new Service(SamlConstants.BindingTypes.HttpPost, "http://localhost:5001/signin-saml")
                    }
                }
            };
        }
    }
}
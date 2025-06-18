using Duende.IdentityServer.Models;
using Duende.IdentityServer;
using Rsk.Saml;
using Rsk.Saml.Models;
using ServiceProvider = Rsk.Saml.Models.ServiceProvider;

namespace DuendeIdP;

public static class Config
{
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        return new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile()
        };
    }

    public static IEnumerable<ApiResource> GetApis()
    {
        return new ApiResource[]
        {
            new ApiResource("api1", "My API #1")
        };
    }

    public static IEnumerable<ApiScope> GetApiScopes()
    {
        return new ApiScope[]
        {
            new ApiScope("scope1"),
            new ApiScope("scope2"),
        };
    }

    public static IEnumerable<Client> GetClients()
    {
        return new[]
        {
            new Client
            {
                ClientId = "https://localhost:5002/saml",
                ClientName = "RSK SAML2P Test Client - Multiple SP",
                ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                AllowedScopes = {"openid", "profile"}
            },
            new Client
            {
                ClientId = "https://localhost:5001/saml",
                ClientName = "RSK SAML2P Test Client - IDP Initiated to SP",
                ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                AllowedScopes = {"openid", "profile"}
            },
            new Client
            {
                ClientId = "https://localhost:5004/saml",
                ClientName = "RSK SAML2P Test Client - IDP Initiated to IDP Dynamic Provider",
                ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                AllowedScopes = {"openid", "profile"}
            }
        };
    }

    public static IEnumerable<ServiceProvider> GetServiceProviders()
    {
        return new[]
        {
            new ServiceProvider
            {
                EntityId = "https://localhost:5002/saml",
                AssertionConsumerServices =
                {
                    new Service(SamlConstants.BindingTypes.HttpPost , "https://localhost:5002/saml/sso"),
                    new Service(SamlConstants.BindingTypes.HttpPost, "https://localhost:5002/signin-saml-3")
                },
                SingleLogoutServices =
                {
                    new Service(SamlConstants.BindingTypes.HttpRedirect , "https://localhost:5002/saml/slo")
                }
            },
            new ServiceProvider
            {
                EntityId = "https://localhost:5001/saml",
                AssertionConsumerServices =
                {
                    new Service(SamlConstants.BindingTypes.HttpPost , "https://localhost:5001/signin-saml")
                },
                SingleLogoutServices =
                {
                    new Service(SamlConstants.BindingTypes.HttpRedirect , "https://localhost:5001/saml/slo")
                },
                AllowIdpInitiatedSso = true,
            },
            new ServiceProvider
            {
                EntityId = "https://localhost:5004/saml",
                AssertionConsumerServices =
                {
                    new Service(SamlConstants.BindingTypes.HttpPost , "https://localhost:5004/federation/saml/signin-saml")
                },
                SingleLogoutServices =
                {
                    new Service(SamlConstants.BindingTypes.HttpRedirect , "https://localhost:5004/saml/slo")
                },
                AllowIdpInitiatedSso = true,
            }
        };
    }
}

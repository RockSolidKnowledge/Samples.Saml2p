using System.Security.Cryptography.X509Certificates;
using Duende.IdentityModel;
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Rsk.Saml;
using Rsk.Saml.Models;
using ServiceProvider = Rsk.Saml.Models.ServiceProvider;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        { 
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource()
            {
                Name = "verification",
                UserClaims = new List<string> 
                { 
                    JwtClaimTypes.Email,
                    JwtClaimTypes.EmailVerified
                }
            }
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        { 
            new ApiScope("api1", "MyAPI") 
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
        { 
        };

    public static IEnumerable<Client> Clients =>
        new List<Client> 
        {
            // machine-to-machine client (from quickstart 1)
            new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },
                
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                // scopes that client has access to
                AllowedScopes = { "api1" },
            },
            // interactive ASP.NET Core Web App
            new Client
            {
                ClientId = "web",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                // where to redirect after login
                RedirectUris = { "https://localhost:5002/signin-oidc" },

                // where to redirect after logout
                PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    "verification"
                }
            },
            new Client()
            {
                ClientId = "https://localhost:5002",
                ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                RedirectUris = new List<string>()
                {
                    "https://localhost:5002/signin-saml"
                },
                
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                }            
            },
            new Client()
            {
                ClientId = "https://localhost:5003",
                ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                RedirectUris = new List<string>()
                {
                    "https://localhost:5003/signin-saml"
                },
                
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                }            
            },
            new Client()
            {
                ClientId = "https://localhost:5004",
                ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                RedirectUris = new List<string>()
                {
                    "https://localhost:5004/signin-saml"
                },
                
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                }            
            }
        };

    public static IEnumerable<ServiceProvider> ServiceProvider => new List<ServiceProvider>()
    {
        new ServiceProvider()
        {
            EntityId = "https://localhost:5002",
            AssertionConsumerServices = new List<Service>()
            {
                new Service(SamlConstants.BindingTypes.HttpPost, "https://localhost:5002/signin-saml")
            },
            SingleLogoutServices = new List<Service>()
            {
              new Service(SamlConstants.BindingTypes.HttpPost, "https://localhost:5002/signout-saml")  
            },
            SigningCertificates = new List<X509Certificate2>()
            {
                new X509Certificate2("Resources/testclient.cer")
            }
        },
        new ServiceProvider()
        {
          EntityId = "https://localhost:5003",
          AssertionConsumerServices = new List<Service>()
          {
              new Service(SamlConstants.BindingTypes.HttpPost, "https://localhost:5003/signin-saml")
          },
          SingleLogoutServices = new List<Service>()
          {
            new Service(SamlConstants.BindingTypes.HttpPost, "https://localhost:5003/signout-saml")  
          },
          SigningCertificates = new List<X509Certificate2>()
          {
              new X509Certificate2("Resources/testclient.cer")
          }
        },
        new ServiceProvider()
        {
            EntityId = "https://localhost:5004",
            AssertionConsumerServices = new List<Service>()
            {
                new Service(SamlConstants.BindingTypes.HttpPost, "https://localhost:5004/signin-saml")
            },
            SingleLogoutServices = new List<Service>()
            {
                new Service(SamlConstants.BindingTypes.HttpPost, "https://localhost:5004/signout-saml")  
            },
            SigningCertificates = new List<X509Certificate2>()
            {
                new X509Certificate2("Resources/testclient.cer")
            }
        }
    };
}
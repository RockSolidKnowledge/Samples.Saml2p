// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Duende.IdentityServer.Models;
using System.Collections.Generic;
using Duende.IdentityServer;
using Rsk.Saml;
using Rsk.Saml.Models;

namespace DuendeDynamicProviders
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
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
                        {new Service(SamlConstants.BindingTypes.HttpPost, "https://localhost:5002/signin-saml-4")}
                }
            };
        }
    }
}
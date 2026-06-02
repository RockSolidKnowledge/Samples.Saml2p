// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Open.IdentityServer;
using Open.IdentityServer.Models;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Rsk.Saml;
using Rsk.Saml.Models;

namespace Open.IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
        [
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        ];
        
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My API")
            };

        public static IEnumerable<Client> Clients =>
        [
            // SAML clients
            new Client
            {
                ClientId = "https://localhost:5001/saml",
                ClientName = "RSK SAML2P Test Client",
                ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
                AllowedScopes = {"openid", "profile"}
            }
        ];

        public static IEnumerable<ServiceProvider> ServiceProviders =>
        [
            new ServiceProvider
            {
                EntityId = "https://localhost:5001/saml",
                AssertionConsumerServices =
                    {new Service(SamlConstants.BindingTypes.HttpPost, "https://localhost:5001/signin-saml")},
                SigningCertificates = {new X509Certificate2("testclient.cer")}
            }
        ];
    }
}
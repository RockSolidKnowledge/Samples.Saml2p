// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.Collections.Generic;
using Duende.IdentityServer;
using IdentityServerHost.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;
using System.Security.Cryptography.X509Certificates;
using Rsk.Saml.DuendeIdentityServer.DynamicProviders;

namespace DuendeDynamicProviders
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var builder = services.AddIdentityServer(options =>
            {
                options.KeyManagement.Enabled = false;
            })
                .AddTestUsers(TestUsers.Users);

            // in-memory, code config
            builder.AddInMemoryIdentityResources(Config.GetIdentityResources());
            builder.AddInMemoryApiScopes(Config.GetApiScopes());
            builder.AddInMemoryClients(Config.GetClients());
            builder.AddSigningCredential(new X509Certificate2("testclient.pfx", "test"));

            // SP configuration - dynamic providers
            builder.AddSamlDynamicProvider(options =>
                {
                    // unstorable/reusable data, such as license information and events. This will override the data stored
                    options.Licensee = "";
                    options.LicenseKey = "";
                })

                // Use EntityFramework store for storing identity providers
                //.AddIdentityProviderStore<SamlIdentityProviderStore>();

                // use in memory store for storing identity providers
                .AddInMemoryIdentityProviders(new List<SamlDynamicIdentityProvider>
                {
                    new SamlDynamicIdentityProvider
                    {
                        SamlAuthenticationOptions = new Saml2pAuthenticationOptions
                        {
                            // The IdP you want to integrate with
                            IdentityProviderOptions = new IdpOptions
                            {
                                EntityId = "https://localhost:5000",
                                SigningCertificates = { new X509Certificate2("idsrv3test.cer") },
                                SingleSignOnEndpoint = new SamlEndpoint("https://localhost:5000/saml/sso", SamlBindingTypes.HttpRedirect),
                                SingleLogoutEndpoint = new SamlEndpoint("https://localhost:5000/saml/slo", SamlBindingTypes.HttpRedirect)
                            },

                            // Details about yourself (the SP)
                            ServiceProviderOptions = new SpOptions
                            {
                                EntityId = "https://localhost:5004/saml",
                                MetadataPath = "/federation/saml/metadata",
                                SignAuthenticationRequests = false // OPTIONAL - use if you want to sign your auth requests
                            },

                            NameIdClaimType = "sub",
                            CallbackPath = "/federation/saml/signin-saml", // Duende prefixes "/federation/{scheme}" to all paths
                            SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
                        },

                        Scheme = "saml",
                        DisplayName = "saml",
                        Enabled = true,
                    }
                });

            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
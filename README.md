# Sample projects implementing Rsk SAML component

SAML2P service provider and identity provider implementations. We support various IdentityServer implementations, including IdentityServer4 and Duende IdentityServer. 

The SAML2P component is available from [www.identityserver.com/products/saml2p](https://www.identityserver.com/products/saml2p).

The [master](https://github.com/RockSolidKnowledge/Samples.IdentityServer4.Saml2pIntegration/tree/master) branch currently uses IdentityServer4 v4 and Duende IdentityServer. Check out [identityserver4-v3](https://github.com/RockSolidKnowledge/Samples.IdentityServer4.Saml2pIntegration/tree/identityserver4-v3) for IdentityServer4 v3 samples.

## Projects

- **spWithIdpInitiated:** an ASP.NET Core website that can accept unsolicited SAML assertions sent via IdP-Initiated SSO
- **spWithMultipleIdps:** an ASP.NET Core website that can authenticate a user using two different SAML Identity Providers

- **IdentityServer4**
	- **idp:** an IdentityServer4 implementation configured to act as a SAML Identity Provider with Service Provider configuration loaded from memory
	- **idpWithEf:** an IdentityServer4 implementation configured to act as a SAML Identity Provider with Service Provider configuration loaded from a database
	- **idpWithIdpInitiated:** an IdentityServer4 implementation configured to send unsolicited SAML assertions using IdP-Initiated SSO
	- **sp:** an IdentityServer4 implementation configured to act as both a SAML Identity Provider and a SAML Service Provider
- **DuendeIdentityServer**
	- **DuendeIdP:** a Duende.IdentityServer implementation configured to act as a SAML Identity Provider with Service Provider configuration loaded from memory. This uses the Duende Automatic Key Management feature for signing key
	- **DuendeDynamicProviders:** A Duende.IdentityServer implementation acting as a SAML Service Provider with in-memory dynamic identity providers


## Getting Started

- [YouTube Tutorials](https://www.youtube.com/playlist?list=PLz9t0GSOz9eCGVZQnQBDg8KH_SRQmzfUm)
- [Gettings Started article](https://www.identityserver.com/articles/saml-20-integration-with-identityserver4/)
- [Documentation](https://www.identityserver.com/documentation/saml2p/)

## License Keys

For a demo license, please sign up on our [products page](https://www.identityserver.com/products/saml2p), or reach out to <sales@identityserver.com>.

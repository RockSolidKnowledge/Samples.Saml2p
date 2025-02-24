# Sample projects implementing Rsk SAML component

SAML2P service provider and identity provider implementations. We support various IdentityServer implementations, including Duende IdentityServer and OpenIddict. 

 The SAML2P component is available from [www.identityserver.com/products/saml2p](https://www.identityserver.com/products/saml2p).


The [master](https://github.com/RockSolidKnowledge/Samples.IdentityServer4.Saml2pIntegration/tree/master) branch currently uses  version 10.0 of the Saml component along with Duende IdentityServer version 7.1 and OpenIddict version 6.0. 

## Projects

- **spWithIdpInitiated:** an ASP.NET Core website that can accept unsolicited SAML assertions sent via IdP-Initiated SSO
- **spWithMultipleIdps:** an ASP.NET Core website that can authenticate a user using two different SAML Identity Providers

- **DuendeIdentityServer**
	- **DuendeIdP:** a Duende.IdentityServer implementation configured to act as a SAML Identity Provider with Service Provider configuration loaded from memory. This uses the Duende Automatic Key Management feature for signing key material. 
	- **DuendeDynamicProviders:** A Duende.IdentityServer implementation acting as a SAML Service Provider with in-memory dynamic identity providers
	- **OpenIddictIdP:** an OpenIddict implementation configured to act as an SAML Identity Provider with Service Provider configuration loaded from an SQL Server database using EntityFrameworkCore. 


## Getting Started

- [YouTube Tutorials](https://www.youtube.com/playlist?list=PLz9t0GSOz9eCGVZQnQBDg8KH_SRQmzfUm)
- [Gettings Started article](https://www.identityserver.com/articles/saml-20-integration-with-identityserver4/)
- [Documentation](https://www.identityserver.com/documentation/saml2p/)

## License Keys
If you are using IdentityServer and would like a demo license, please sign up on our [products page](	https://www.identityserver.com/products/saml2p). Alternative for a demo license using OpenIddict use [this](https://www.openiddictcomponents.com/products/saml2p) link. You can also reach out to <sales@identityserver.com>.


#Launch urls
The launch urls for the sample projects are as follows:
|Project|Launch Url|
|---|---|
|duende.idp|https://localhost:5003|
|OpenIddict.Idp|https://localhost:55031|
|Duende Dynamic providers|https://localhost:5004|
|SPWithIdpInitiated|https://localhost:5001|
|SpWithMultipleIds|http/s://localhost:5002|
|SpWithSingleIdp|https://localhost:5002|
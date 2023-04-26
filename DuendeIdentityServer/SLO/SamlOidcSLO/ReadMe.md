# Sample projects implementing Single Logout using the Rsk SAML component with Iterative SLO

This sample project shows how to configure SAML SLO using the traditional iterative approach. This way of implementing SLO does not require cross-site cookies and will continue to work when cross-site cookies are blocked by browsers.

## Projects

The solution contains multiple applications using both OIDC and SAML clients to simulate a production environment.

- **IdentityServer:** A Duende IdentityServer quickstart configured to use the SAML2P component
- **WebClient.OIDC:** an ASP.NET Core website that can authenticate a user using the OIDC protocol
- **WebClient.SAML.1:** an ASP.NET Core website configured with the SAML2P compoent can authenticate a user using the SAML protocol
- **WebClient.SAML.1:** an ASP.NET Core website configured with the SAML2P compoent can authenticate a user using the SAML protocol

These projects all run on localhost in this sample, to get around cookie collisions, each project is configured with a different cookie name. This is not required in a production environment when each application is running under a different address. As each application is running on localhost the IdentityServer iFrame SLO will fail for the OIDC client when a logout is started from a SAML client.

## License Keys

This project requires a valid SAML2P license. Insert your License into the Common.TestConstants file.

For a demo license, please sign up on our [products page](https://www.identityserver.com/products/saml2p), or reach out to <sales@identityserver.com>.
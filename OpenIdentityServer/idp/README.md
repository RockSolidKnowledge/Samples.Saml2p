# OpenIdentityServer SAML2P Identity Provider

This sample demonstrates a **SAML2P Identity Provider** built with **Open.IdentityServer** using the **Rsk.Saml.OpenIdentityServer** component (version 11.2.0). It showcases how to configure and run an OpenIdentityServer instance as a SAML Identity Provider that can authenticate and issue SAML assertions for service providers.

## Sample Features

- **SAML2P Identity Provider** - OpenIdentityServer configured as a SAML2p compliant IdP
- **Multiple Service Providers** - Support for multiple SAML service providers
- **SAML Metadata Endpoint** - Automatically generates and serves SAML metadata
- **SAML Assertions** - Issues signed SAML assertions for authenticated users
- **Configurable Clients** - In-memory configuration of SAML clients and service providers

## Architecture

This Identity Provider consists of:

- **Authorization Server** - Open.IdentityServer instance handling authentication
- **SAML Plugin** - Rsk.Saml middleware for SAML2P protocol support
- **Service Provider Registry** - In-memory configuration of trusted service providers
- **User Database** - Test users for authentication

## Configuration Overview

The SAML plugin is configured in `Startup.cs` with the following components:

```csharp
services.AddIdentityServer()
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryClients(Config.Clients)
    .AddTestUsers(TestUsers.Users)
    .AddSamlPlugin(options =>
    {
        options.Licensee = "DEMO";
        options.LicenseKey = "<insert here>";
    })
    .AddInMemoryServiceProviders(Config.ServiceProviders);

builder.AddSigningCredential(X509CertificateLoader.LoadPkcs12FromFile("testclient.pfx", "test"));
```

The middleware is activated with:

```csharp
app.UseIdentityServer()
    .UseIdentityServerSamlPlugin();
```

## Configuring SAML Clients

Each service provider that will authenticate with this IdP must be registered as a SAML client in `Config.cs`:

```csharp
public static IEnumerable<Client> Clients =>
[
    new Client
    {
        ClientId = "https://localhost:5001/saml",
        ClientName = "My Service Provider",
        ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
        AllowedScopes = { "openid", "profile" }
    },
    new Client
    {
        ClientId = "https://localhost:5002/saml",
        ClientName = "Another Service Provider",
        ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
        AllowedScopes = { "openid", "profile" }
    }
];
```

**Client Configuration Details:**

- **ClientId**: Must match the Service Provider's Entity ID (e.g., `https://localhost:5001/saml`)
- **ProtocolType**: Set to `IdentityServerConstants.ProtocolTypes.Saml2p` for SAML providers
- **AllowedScopes**: Define which identity scopes the SP can request (e.g., `openid`, `profile`)

## Registering Service Providers

Each service provider must be registered with its assertion consumer service (ACS) endpoint and signing certificate in `Config.cs`:

```csharp
public static IEnumerable<ServiceProvider> ServiceProviders =>
[
    new ServiceProvider
    {
        EntityId = "https://localhost:5001/saml",
        AssertionConsumerServices =
        {
            new Service(
                SamlConstants.BindingTypes.HttpPost,
                "https://localhost:5001/signin-saml"
            )
        },
        SigningCertificates = { new X509Certificate2("testclient.cer") }
    },
    new ServiceProvider
    {
        EntityId = "https://localhost:5002/saml",
        AssertionConsumerServices =
        {
            new Service(
                SamlConstants.BindingTypes.HttpPost,
                "https://localhost:5002/signin-saml"
            )
        },
        SigningCertificates = { new X509Certificate2("testclient.cer") }
    }
];
```

**Service Provider Configuration Details:**

- **EntityId**: Must match the SP's Entity ID and the Client ID
- **AssertionConsumerServices**: The ACS endpoint(s) where SAML assertions will be sent (POST binding)
- **SigningCertificates**: The public certificate used to verify SP-signed requests (optional for some flows)

## SAML Metadata

The Identity Provider publishes SAML metadata at:

```
https://localhost:5009/saml/metadata
```

Service providers will use this metadata to:
- Discover the IdP's single sign-on (SSO) endpoint
- Obtain the IdP's signing certificate
- Validate SAML assertions

Provide this metadata URL to service providers during configuration.

## Getting Started

### 1. Configure License

Update the license information in `Startup.cs`:

```csharp
options.Licensee = "<insert here>";
options.LicenseKey = "<insert here>";
```

Get a license key from [identityserver.com/products/saml2p](https://www.identityserver.com/products/saml2p) or use the demo license for testing.

### 2. Add Service Providers

Register service providers in `Config.cs`:

```csharp
public static IEnumerable<ServiceProvider> ServiceProviders =>
[
    new ServiceProvider
    {
        EntityId = "https://your-sp-domain/saml",
        AssertionConsumerServices =
        {
            new Service(
                SamlConstants.BindingTypes.HttpPost,
                "https://your-sp-domain/signin-saml"
            )
        },
        SigningCertificates = { new X509Certificate2("your-sp-certificate.cer") }
    }
];
```

### 3. Add SAML Clients

Register SAML clients in `Config.cs`:

```csharp
public static IEnumerable<Client> Clients =>
[
    new Client
    {
        ClientId = "https://your-sp-domain/saml",
        ClientName = "Your Service Provider",
        ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p,
        AllowedScopes = { "openid", "profile" }
    }
];
```

### 4. Configure Signing Credentials

Update the signing certificate in `Startup.cs`. Currently it uses:

```csharp
builder.AddSigningCredential(X509CertificateLoader.LoadPkcs12FromFile("testclient.pfx", "test"));
```

For production, use a proper certificate:

```csharp
builder.AddSigningCredential(X509CertificateLoader.LoadPkcs12FromFile("your-certificate.pfx", "password"));
```

### 5. Run the Identity Provider

```
dotnet run
```

The Identity Provider will be available at `https://localhost:5009`

### 6. Test SSO

1. Navigate to the Service Provider
2. Initiate SAML authentication (SP-Initiated or IdP-Initiated SSO)
3. You will be redirected to `https://localhost:5009` to authenticate
4. After successful authentication, you will be redirected back to the SP

## Test Users

The sample includes test users from `TestUsers.Users`. Commonly used credentials:

- Username: `alice` / Password: `alice` (with profile claims)
- Username: `bob` / Password: `bob` (with profile claims)

## Project Structure

- **Startup.cs** - Middleware and service configuration
- **Config.cs** - SAML clients and service provider configuration
- **testclient.pfx** - Signing certificate (private key, development only)
- **testclient.cer** - Public certificate (for service providers)
- **Controllers/** - Application controllers
- **Quickstart/** - UI quickstart views (login, consent, logout)
- **Views/** - Razor views

## Documentation

For more information about SAML2P configuration with OpenIdentityServer and Rsk.Saml:

📖 **[Rsk.Saml Documentation](https://docs.identityserver.com/saml2p/)**

📖 **[IdentityServer as SAML2P Identity Provider Quickstart](https://docs.identityserver.com/saml2p/quickstarts/identityServer/identityServerAsIdp/)**

📖 **[Open.IdentityServer Documentation](https://docs.openidentityserver.dev/)**

## Notes

- This is a **development sample** using test certificates and in-memory configuration
- For production, use proper certificates and persistent storage for configuration
- Always use HTTPS in production
- The test certificate `testclient.pfx` should not be used in production
- License key in Startup.cs is for demonstration purposes only - obtain a proper license key
- The sample uses in-memory storage - consider using database storage for production

## Support

For technical issues or questions:

- Check the [Rsk.Saml Documentation](https://docs.identityserver.com/saml2p/)
- Review the [IdentityServer SAML2P Quickstart](https://docs.identityserver.com/saml2p/quickstarts/identityServer/identityServerAsIdp/)
- Contact support at [identityserver.com](https://www.identityserver.com)

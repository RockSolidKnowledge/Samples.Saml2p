# Service Provider with Multiple SAML Identity Providers

This sample demonstrates a **standalone ASP.NET Core Service Provider** using the **Rsk.Saml** component (version 11.2.0). It showcases how to configure multiple SAML2 authentication schemes, each representing a distinct SAML identity provider integration.

## Sample Features

- **Multiple SAML Schemes** - Configure multiple independent SAML2p authentication schemes
- **SP-Initiated SSO** - User-initiated login flows where the SP initiates the authentication request  
- **IdP-Initiated SSO** - Identity provider-initiated login flows where the IdP can initiate authentication
- **Configurable Metadata Endpoints** - Unique SAML metadata for each scheme
- **Multiple Identity Providers** - Support multiple SAML identity providers in a single application

## Configuration Overview

Each SAML scheme is configured independently in `Startup.cs` with the following key components:

## Multiple Authentication Schemes

```csharp
builder.Services.AddAuthentication()
    .AddCookie("cookie")
    .AddSaml2p("saml-1", options => {
        options.Licensee = "<insert here>";
        options.LicenseKey = "<insert here>";
        options.IdentityProviderMetadataAddress = "<IdP metadata URL>";
        options.ServiceProviderOptions = new SpOptions {
            EntityId = "https://localhost:5001/saml",
            MetadataPath = "/saml/metadata-1"
        };
        options.CallbackPath = "/signin-saml-1";
        options.SignInScheme = "cookie";
        options.AllowIdpInitiatedSso = true;
        options.IdPInitiatedSsoCompletionPath = "/";
    })
    .AddSaml2p("saml-2", options => {
        // ... similar configuration with different paths
        options.MetadataPath = "/saml/metadata-2";
        options.CallbackPath = "/signin-saml-2";
    });
```

### Key Configuration Points

**Scheme Names**: Each scheme (`"saml-1"`, `"saml-2"`, etc.) uniquely identifies a SAML authentication provider.

**Callback Paths**: Each scheme requires its own unique callback path:
- `saml-1` uses `/signin-saml-1`
- `saml-2` uses `/signin-saml-2`

These are the Assertion Consumer Service (ACS) endpoints where your IdP will send SAML responses.

**Metadata Paths**: Each service provider exposes its own SAML metadata:
- `saml-1` uses `/saml/metadata-1`
- `saml-2` uses `/saml/metadata-2`

The IdP must be configured with the correct metadata endpoint for each scheme.

**Cookie Scheme**: Both SAML schemes use `"cookie"` as the `SignInScheme`, meaning authenticated users are stored in a session cookie.

## Configuring Your Identity Provider

To integrate this Service Provider with your SAML Identity Provider, configure the following:

### 1. Register the Service Provider

Register this Service Provider in your Identity Provider's configuration:

- **Entity ID**: `https://localhost:5001/saml`
- **Assertion Consumer Service (ACS) Endpoint**: `https://localhost:5001/signin-saml-1`
- **Second ACS Endpoint** (for second scheme): `https://localhost:5001/signin-saml-2`
- **Signing Certificate**: Upload the public certificate from `idsrv3test.cer` (or your production certificate)
- **Metadata URL**: `https://localhost:5001/saml/metadata-1` and `https://localhost:5001/saml/metadata-2`

Each SAML scheme requires its own Service Provider configuration with its unique ACS endpoint.

### 2. IdP-Initiated SSO (Optional)

If your Identity Provider supports unsolicited SAML assertions (IdP-Initiated SSO), configure it to send SAML responses directly to the ACS endpoints (`https://localhost:5001/signin-saml-1` or `https://localhost:5001/signin-saml-2`) without requiring an authentication request first.

## Getting Started

1. **Configure License**:
   ```csharp
   options.Licensee = "<insert here>";
   options.LicenseKey = "<insert here>";
   ```
   Get a license key from [identityserver.com/products/saml2p](https://www.identityserver.com/products/saml2p)

2. **Configure IdP Metadata Address**:
   ```csharp
   options.IdentityProviderMetadataAddress = "https://your-idp.com/saml/metadata";
   ```

3. **Configure Your IdP** - Register this Service Provider in your Identity Provider (see "Configuring Your Identity Provider" section above)

4. **Download SAML Metadata** - Available at:
   - `https://localhost:5001/saml/metadata-1`
   - `https://localhost:5001/saml/metadata-2`

5. **Run the Sample**:
   ```
   dotnet run
   ```
   The application will be available at `https://localhost:5001`

## Project Structure

- **Startup.cs** - SAML authentication scheme configuration
- **Controllers/** - Application controllers
- **Views/** - Razor views for the UI
- **idsrv3test.cer** - Test certificate (development only)

## Documentation

For more information about SAML2p integration:

📖 **[Rsk.Saml Documentation](https://docs.identityserver.com/saml2p/)**

📖 **[IdentityServer as SAML IdP Quickstart](https://docs.identityserver.com/saml2p/quickstarts/identityServer/identityServerAsIdp/)**

## Notes

- This is a **development sample** using test certificates
- Always use proper certificates in production
- The test certificate `idsrv3test.cer` is for demonstration only
- Ensure your IdP is configured with the correct SP Entity ID and ACS endpoints for each scheme







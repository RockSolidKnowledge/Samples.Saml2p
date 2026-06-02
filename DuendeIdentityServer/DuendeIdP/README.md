# Duende IdentityServer SAML2P Identity Provider Sample

This sample shows **Duende IdentityServer** configured as a **SAML2P Identity Provider (IdP)** using `Rsk.Saml.DuendeIdentityServer`.

## Sample Features

- Duende IdentityServer running as a SAML IdP
- In-memory SAML clients and service provider registrations
- SAML SSO + SLO endpoint configuration
- Test user sign-in flow
- Automatic key management for signing keys

## Project Layout

- `Program.cs` - host/bootstrap and Serilog setup
- `HostingExtensions.cs` - IdentityServer + SAML plugin configuration
- `Config.cs` - in-memory clients, resources, and service providers

## Configuration Overview

The SAML plugin is configured in `HostingExtensions.cs`:

```csharp
isBuilder.AddSamlPlugin(options =>
    {
        options.Licensee = "<insert here>";
        options.LicenseKey = "<insert here>";
        options.WantAuthenticationRequestsSigned = false;
    })
    .AddInMemoryServiceProviders(Config.GetServiceProviders());
```

Update `Licensee` and `LicenseKey` before running.

## Clients and Service Providers

Current in-memory sample values in `Config.cs`:

- SAML client `ClientId`: `https://localhost:5002/saml`
- Service provider `EntityId`: `https://localhost:5002/saml`
- ACS endpoints:
  - `https://localhost:5002/saml/sso`
  - `https://localhost:5002/signin-saml-3`
- SLO endpoint:
  - `https://localhost:5002/saml/slo`

If you add another service provider:

1. Add a `Client` with `ProtocolType = IdentityServerConstants.ProtocolTypes.Saml2p`
2. Add matching `ServiceProvider` configuration with ACS/SLO endpoints
3. Ensure `ClientId` and `EntityId` are aligned

## Running the Sample

From repo root:

```powershell
dotnet run --project .\DuendeIdentityServer\DuendeIdP\DuendeIdP.csproj
```

This sample uses default ASP.NET Core hosting behavior for URL/port unless set by launch/profile or environment.

## Documentation

- Rsk.Saml docs: <https://docs.identityserver.com/saml2p/>
- IdentityServer as SAML IdP quickstart: <https://docs.identityserver.com/saml2p/quickstarts/identityServer/identityServerAsIdp/>
- Duende IdentityServer docs: <https://docs.duendesoftware.com/identityserver/>

## Notes

- This is a development sample using in-memory configuration.
- Use production certificates/secrets and persistent storage for real deployments.
- Replace placeholder license values before startup.


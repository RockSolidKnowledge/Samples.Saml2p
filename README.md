# Samples.Saml2p

This repository contains sample **Service Provider (SP)** and **Identity Provider (IdP)** implementations using the **Rsk.Saml** component with different IdentityServer-based stacks.

## What is in this repo

- Standalone SP samples using `Rsk.Saml` for ASP.NET Core
- IdP samples using:
  - `Duende.IdentityServer`
  - `Open.IdentityServer`
  - `OpenIddict`
- Examples for:
  - SP-initiated SSO
  - IdP-initiated SSO
  - Multiple SAML schemes / multiple IdPs
  - Dynamic provider scenarios

## Projects

### Service Provider samples

- `ServiceProvider/` - standalone SP sample using `Rsk.Saml`
  - multiple SAML schemes
  - per-scheme callback paths
  - per-scheme metadata paths
  - IdP-initiated SSO support

### Duende IdentityServer samples

- `DuendeIdentityServer/DuendeIdP/` - Duende IdentityServer as a SAML IdP
- `DuendeIdentityServer/DuendeDynamicProviders/` - Duende IdentityServer as SP with dynamic providers
- `DuendeIdentityServer/SLO/SamlOidcSLO/` - sample focused on SLO scenarios

### Open.IdentityServer samples

- `OpenIdentityServer/idp/` - Open.IdentityServer configured as a SAML IdP
- `OpenIdentityServer/idpWithEf/` - Open.IdentityServer IdP with EF-backed configuration
- `OpenIdentityServer/idpWithIdpInitiated/` - Open.IdentityServer IdP with IdP-initiated flow scenario
- `OpenIdentityServer/sp/` - Open.IdentityServer-related SP sample

### IdentityServer4 legacy samples

- `IdentityServer4/idp/`
- `IdentityServer4/idpWithEf/`
- `IdentityServer4/idpWithIdpInitiated/`
- `IdentityServer4/sp/`

### OpenIddict sample

- `OpenIddict/OpenIddictIdP/` - OpenIddict-based SAML IdP sample

## Documentation

- Rsk.Saml docs: <https://docs.identityserver.com/saml2p/>
- IdentityServer as SAML IdP quickstart: <https://docs.identityserver.com/saml2p/quickstarts/identityServer/identityServerAsIdp/>
- Product page / licensing: <https://www.identityserver.com/products/saml2p>

## Getting Started

1. Pick a sample folder and open its local `README.md` first (for sample-specific setup).
2. Add license values where required (`Licensee`, `LicenseKey`).
3. Confirm certificate files used by that sample (`.cer` / `.pfx`).
4. Start the selected sample project.

Example run command:

```powershell
dotnet run --project .\ServiceProvider\ServiceProvider.csproj
```

> Note: launch URLs and runtime details differ by sample. Use each project README and launch settings as the source of truth.

## License Keys

For demo or production licensing, use the product page above or contact `sales@identityserver.com`.

# OpenIddict SAML2P Identity Provider Sample

This sample shows how to run **OpenIddict** as a **SAML2P Identity Provider (IdP)** using the `Rsk.Saml.OpenIddict` components.

## Sample Features

- OpenIddict server with SAML2P plugin enabled
- ASP.NET Core Identity for user authentication
- EF Core-backed SAML/OpenIddict configuration
- Multiple database provider options (`PostgreSql`, `MySql`, `SqlServer`, `Sqlite`)
- Seeded SAML clients/service providers for local testing
- Quartz background jobs for SAML message pruning

## Project Layout

- `Startup.cs` - OpenIddict + SAML plugin registration and middleware setup
- `Worker.cs` - database migration + seed data for clients/scopes/service providers/users
- `appsettings.json` - database provider and connection string selection
- `DbProvider.cs` - supported provider enum
- `Resources/` - test certificates used by seeded service providers

## Configuration Overview

### OpenIddict + SAML registration

The sample registers OpenIddict and enables SAML support via `AddSamlPlugin(...)` in `Startup.cs`.

Key points in current setup:

- OpenIddict endpoints:
  - `connect/authorize`
  - `connect/logout`
  - `connect/token`
  - `connect/userinfo`
- SAML IdP options configured in `Startup.cs` (`Licensee`, `LicenseKey`, `UseIFramesForSlo`)
- SAML middleware enabled with `app.UseOpenIddictSamlPlugin()`

### License values

Update these placeholders in `Startup.cs` before running:

```csharp
serverOptions.IdpOptions = new SamlIdpOptions()
{
    Licensee = "<insert here>",
    LicenseKey = "<insert here>",
    UseIFramesForSlo = false
};
```

## Database Provider Options

`appsettings.json` controls both the active provider and connection string:

```json
"DbProvider": "PostgreSql"
```

Supported values:

- `PostgreSql`
- `MySql`
- `SqlServer`
- `Sqlite`

## Local Database Quick Start (Docker)

### PostgreSQL

```powershell
docker run --name my-postgres -e POSTGRES_USER=myuser -e POSTGRES_PASSWORD=mypassword -e POSTGRES_DB=mydb -p 5432:5432 -d postgres:latest
```

Connection string:

```text
Host=localhost;Port=5432;Database=mydb;Username=myuser;Password=mypassword
```

### MySQL

```powershell
docker run --name my-mysql -e MYSQL_ROOT_PASSWORD=mypassword -e MYSQL_DATABASE=mydb -e MYSQL_USER=myuser -e MYSQL_PASSWORD=mypassword -p 3306:3306 -d mysql:latest
```

Connection string:

```text
Server=localhost;Port=3306;Database=mydb;User=myuser;Password=mypassword
```

### SQL Server

```powershell
docker run --name my-mssql -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=MyStrong!Passw0rd" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

Connection string:

```text
Server=localhost,1433;Database=master;User Id=sa;Password=MyStrong!Passw0rd;TrustServerCertificate=True
```

You can adjust usernames, passwords, ports, and database names as needed.

## Seeded Data

`Worker.cs` applies migrations and seeds initial data on startup.

### Seeded OpenIddict clients

- `mvc`
- `https://localhost:5001/saml`

### Seeded SAML service providers

- `https://localhost:5001/saml`
  - ACS: `https://localhost:5001/signin-saml-openIddict`
- `https://localhost:5001/saml/artifact`
  - ACS: `https://localhost:5001/signin-saml-openIddict-artifact`

### Seeded user

- Username: `bob@test.fake`
- Password: `Password123!`

## Running the Sample

1. Choose a database provider in `appsettings.json`.
2. Set the matching `DefaultConnection` string.
3. Add valid SAML license values in `Startup.cs`.
4. Run the project.

```powershell
dotnet run --project .\OpenIddict\OpenIddictIdP\OpenIddictIdP.csproj
```

This sample does not include a `launchSettings.json` in the current project folder, so URL/port will depend on your host defaults or environment settings.

## Documentation

- Rsk.Saml docs: <https://docs.identityserver.com/saml2p/>
- Product page / licensing: <https://www.identityserver.com/products/saml2p>
- OpenIddict docs: <https://documentation.openiddict.com/>

For conceptual SAML IdP setup flow, this quickstart is also useful:

- <https://docs.identityserver.com/saml2p/quickstarts/identityServer/identityServerAsIdp/>

## Notes

- This is a development sample using seeded users and test certificates.
- `Worker.cs` performs schema creation/migrations and seed operations automatically at startup.
- For production, use dedicated certificates, secure secrets management, and production-grade DB settings.

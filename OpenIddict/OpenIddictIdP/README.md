# OpenIddict IDP sample

To run the project with a specific database provider, the following commands and connection strings can be used to run with the database servers in Docker

#### PostgreSQL

```sh
docker run --name my-postgres -e POSTGRES_USER=myuser -e POSTGRES_PASSWORD=mypassword -e POSTGRES_DB=mydb -p 5432:5432 -d postgres:latest
```

Connection string:
```
Host=localhost;Port=5432;Database=mydb;Username=myuser;Password=mypassword
```

#### MySQL

```sh
docker run --name my-mysql -e MYSQL_ROOT_PASSWORD=mypassword -e MYSQL_DATABASE=mydb -e MYSQL_USER=myuser -e MYSQL_PASSWORD=mypassword -p 3306:3306 -d mysql:latest
```

Connection string:
```
Server=localhost;Port=3306;Database=mydb;User=myuser;Password=mypassword
```

#### SQL Server

```sh
docker run --name my-mssql -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=MyStrong!Passw0rd' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

Connection string:
```
Server=localhost,1433;Database=master;User Id=sa;Password=MyStrong!Passw0rd;TrustServerCertificate=True
```

You can adjust usernames, passwords, and database names as needed.
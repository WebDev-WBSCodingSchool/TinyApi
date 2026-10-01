# TinyApi

A minimal ASP.NET Core API on .NET 10 that connects to SQL Server. It is used to practise Docker and Docker Compose.

## What it contains

- `Program.cs` with two endpoints:
  - `GET /` returns a greeting
  - `GET /db-check` runs `SELECT SYSDATETIMEOFFSET()` on SQL Server and returns the server time
- An empty `AppDbContext` using `Microsoft.EntityFrameworkCore.SqlServer` 10.0.x. When `ApplyMigrations` is `true` (the default), the app calls `MigrateAsync` on startup, which creates the database if it does not exist
- `Dockerfile`: a multi-stage build with `sdk:10.0` for publishing and `aspnet:10.0` for running
- `docker-compose.yaml`: SQL Server 2022 (`db`) and the API (`api`). The API waits until the database passes its health check
- `TinyApi.http`: requests for both endpoints

## Run it with Docker Compose

Requires Docker Desktop (or Docker Engine with the Compose plugin).

```bash
docker compose up --build
```

The first start downloads the SQL Server image and can take a few minutes. Then:

```bash
curl http://localhost:8080/
curl http://localhost:8080/db-check
```

Stop and remove the containers:

```bash
docker compose down
```

Add `-v` to also delete the database volume.

## Configuration

| Setting | Where it is set | Purpose |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | `docker-compose.yaml` | Connection string for SQL Server. The double underscore maps to `ConnectionStrings:DefaultConnection` |
| `ApplyMigrations` | `docker-compose.yaml` | Set to `false` to skip `MigrateAsync` on startup |
| `MSSQL_SA_PASSWORD` | `docker-compose.yaml` | Password for the SQL Server `sa` user |

The password in `docker-compose.yaml` is for local development only. Do not reuse it anywhere else.

## Run the API without Docker

Start only the database, then run the API with the connection string as an environment variable:

```bash
docker compose up -d db

ConnectionStrings__DefaultConnection="Server=localhost,1433;Database=TinyDb;User Id=sa;Password=Your_password123;TrustServerCertificate=true" \
  dotnet run
```

The API then listens on `http://localhost:5065`.

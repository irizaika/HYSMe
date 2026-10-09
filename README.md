# HYSMe

Solution overview
- Multi-project .NET 10 solution for a pet sighting application.
- Projects:
  - WebApp — web UI (MVC / Razor views)
  - PetsApi — REST API for pets and sightings (SQLite by default)
  - AuthApi — authentication API
  - Contracts — shared DTOs/contracts
  - Tests — unit and integration tests under respective projects

Prerequisites
- .NET 10 SDK
- Optional: Docker

Build
 - dotnet build

Run (development)
- From Visual Studio: open `HYSMe.sln` and run the desired project.
- From CLI:
  - Run the API (example): `cd PetsApi && dotnet run`
  - Run the web UI: `cd WebApp && dotnet run`

Configuration
- Configuration files are in each project (appsettings.json / appsettings.Development.json).
- Important settings:
  - ConnectionStrings:DefaultConnection — SQLite file path by default (`pet.db`).
  - ApiSettings:Secret, Issuer, Audience — JWT settings used by APIs.
  - ServiceUrls (WebApp) — Service URL endpoints for WebApp to call APIs: `ServiceUrls:API`, `ServiceUrls:AuthAPI`.

Secrets
- Do NOT keep production secrets in source control. The repo currently contains a development JWT secret in `PetsApi/appsettings.Development.json`.
- Recommended: use environment variables, user-secrets for local development, or a secret store (Azure Key Vault, AWS Secrets Manager) in production.

Tests
- Run tests with: `dotnet test` from the solution root or run individual test projects.

Docker
- Each project provides a Dockerfile. Example build and run for PetsApi:
  - docker build -f PetsApi/dockerfile -t hysme/petsapi:local .
  - docker run -e "ASPNETCORE_ENVIRONMENT=Development" -p 5001:80 hysme/petsapi:local

Notes and Recommendations
- Secrets: move JWT secret out of checked-in config and load from environment or secret store.
- Database: SQLite is acceptable for development; choose a production-grade DB (Postgres, SQL Server) for production.
- CI/CD: add a pipeline that runs `dotnet build` and `dotnet test` on PRs.
- Logging & monitoring: add structured logging (Serilog), health checks, and metrics.
- Security: review JWT and cookie authentication settings, enable HSTS in production, and restrict Swagger to non-production environments.

Contact / Contribution
- Open issues and pull requests are welcome. Add a CONTRIBUTING.md if you want contribution rules.

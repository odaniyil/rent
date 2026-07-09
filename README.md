# Rent-A-Home

Rent-A-Home is a property rental management platform for leasing semi-furnished apartments from owners, furnishing them, and sub-leasing them to tenants.

## Structure

- `backend/` - .NET 10 ASP.NET Core API using a Clean Architecture project layout.
- `frontend/management-web/` - React, TypeScript, Vite, and Tailwind CSS management portal.
- `mobile/tenant-app/` - Placeholder for the future tenant Expo app.
- `mobile/field-app/` - Placeholder for the future field team Expo app.
- `contracts/` - Placeholder for shared API contracts.
- `docs/` - Architecture and project documentation.
- `infrastructure/` - Placeholder for deployment and infrastructure setup.

## Prerequisites

- .NET 10 SDK
- Node.js and npm
- Docker with Docker Compose

## PostgreSQL

Copy `.env.example` to `.env` and adjust local values if needed.

```bash
docker compose up -d postgres
```

The default local database is `rentahome_dev`. The backend development connection string matches the default Compose credentials. Override it with `ConnectionStrings__DefaultConnection` if you change database credentials.

If local port `5432` is already in use, set another port in `.env`:

```bash
POSTGRES_PORT=5433
```

When using a non-default port, pass the matching connection string to EF Core and the backend:

```bash
export ConnectionStrings__DefaultConnection="Host=localhost;Port=5433;Database=rentahome_dev;Username=rentahome;Password=rentahome_dev_password"
```

## Database Migrations

Restore the repo-local EF Core tool and apply migrations:

```bash
dotnet tool restore
dotnet dotnet-ef database update \
  --project backend/src/RentAHome.Persistence/RentAHome.Persistence.csproj \
  --startup-project backend/src/RentAHome.Api/RentAHome.Api.csproj
```

Create a new migration after domain model changes:

```bash
dotnet dotnet-ef migrations add MigrationName \
  --project backend/src/RentAHome.Persistence/RentAHome.Persistence.csproj \
  --startup-project backend/src/RentAHome.Api/RentAHome.Api.csproj \
  --output-dir Migrations
```

## Backend

```bash
cd backend
dotnet restore RentAHome.slnx
dotnet build RentAHome.slnx
dotnet run --project src/RentAHome.Api/RentAHome.Api.csproj
```

The API exposes:

- `GET /health`
- `POST /api/auth/login`
- `GET /api/test/protected`
- `GET /api/test/super-admin`
- `GET /api/properties`
- `POST /api/properties`
- `GET /api/properties/{id}`
- `PUT /api/properties/{id}`
- `DELETE /api/properties/{id}`
- OpenAPI JSON at `/openapi/v1.json` in development

## Authentication

Development JWT settings live in `backend/src/RentAHome.Api/appsettings.Development.json`. For non-development environments, set `Jwt__SigningKey` from a secret store or environment variable.

The database seed creates one demo SuperAdmin account:

- Email: `superadmin@rentahome.local`
- Password: `ChangeMe123!`

Login example:

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"superadmin@rentahome.local","password":"ChangeMe123!"}'
```

Use the returned `accessToken` as a bearer token for protected endpoints:

```bash
curl http://localhost:5000/api/test/super-admin \
  -H "Authorization: Bearer <accessToken>"
```

## Property APIs

Property APIs require a bearer token. List filters are optional:

```bash
curl "http://localhost:5000/api/properties?city=San%20Francisco&locality=SoMa&status=Leased&occupancy=Occupied&leaseExpiringBefore=2027-12-31" \
  -H "Authorization: Bearer <accessToken>"
```

Property detail includes active owner lease, active tenant lease, inventory count, maintenance count, and a placeholder profitability summary based on active lease rents.

## Frontend

```bash
cd frontend/management-web
npm install
npm run dev
```

For a production build:

```bash
npm run build
```

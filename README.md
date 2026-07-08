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

The default local database is `rentahome_dev`.

## Backend

```bash
cd backend
dotnet restore RentAHome.slnx
dotnet build RentAHome.slnx
dotnet run --project src/RentAHome.Api/RentAHome.Api.csproj
```

The API exposes:

- `GET /health`
- OpenAPI JSON at `/openapi/v1.json` in development

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

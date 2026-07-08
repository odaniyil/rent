# Architecture

Rent-A-Home uses a Clean Architecture backend split into API, Application, Domain, Infrastructure, and Persistence projects.

- `RentAHome.Api` hosts HTTP endpoints, OpenAPI, request/response contracts, and composition root wiring.
- `RentAHome.Application` will contain use cases, interfaces, DTOs, validation, and business orchestration.
- `RentAHome.Domain` will contain core business entities, value objects, and domain rules.
- `RentAHome.Infrastructure` will contain external adapters such as notifications, storage, mock payment providers, and future integrations.
- `RentAHome.Persistence` will contain Entity Framework Core database access and migrations.

The management portal is a separate React application that will call the backend API through shared contracts once the contract package is introduced.

Tenant and field team mobile apps are intentionally placeholders in this foundation task. They should be scaffolded when their first concrete workflow is defined.

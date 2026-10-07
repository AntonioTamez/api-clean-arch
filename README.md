# ApiCleanArch

A .NET 10 / C# 14 Web API that exposes a `Users` CRUD using Clean Architecture (hexagonal ports and adapters).
Data lives in an in-memory, seeded store that behaves like a database: it persists primitive records and rehydrates domain entities on every read.

## Layers

| Project | Role | References |
| --- | --- | --- |
| `ApiCleanArch.Domain` | Entities, value objects, domain errors. No dependencies. | none |
| `ApiCleanArch.Application` | Use-case handlers, DTOs, ports (`IUserRepository`), application errors. | Domain |
| `ApiCleanArch.Infrastructure` | Adapters implementing the ports (in-memory repository). | Application |
| `ApiCleanArch.Api` | HTTP controllers, request contracts, error handling, DI composition root. | Application, Infrastructure |

Each project is organized by feature (`Users/`).

Dependency rule: dependencies point inward only (`Api -> Application -> Domain`, `Infrastructure -> Application`).
The domain model is rich: `User` guards its invariants through value objects (`UserId`, `Email`, `FullName`) and behavior methods (`Rename`, `ChangeEmail`), with private setters and factory methods (`Create`, `Rehydrate`).

## Swapping the in-memory store for a database

1. Add a repository class in `Infrastructure` (for example `EfUserRepository`) implementing `IUserRepository`, mapping between `User` and its persistence model.
2. Change the single registration in `src/ApiCleanArch.Infrastructure/DependencyInjection.cs`.

Domain, Application and Api remain untouched.

## Run

```bash
dotnet run --project src/ApiCleanArch.Api
```

The OpenAPI document is served at `/openapi/v1.json` in Development.

## Test

```bash
dotnet test
```

## Endpoints

| Method | Route | Success | Errors |
| --- | --- | --- | --- |
| GET | `/api/v1/users` | 200 list | |
| GET | `/api/v1/users/{id}` | 200 | 404 |
| POST | `/api/v1/users` | 201 + `Location` | 400, 409 |
| PUT | `/api/v1/users/{id}` | 200 | 400, 404, 409 |
| DELETE | `/api/v1/users/{id}` | 204 | 404 |

Request body for POST and PUT: `{ "name": "Ada Lovelace", "email": "ada@example.com" }`.
Errors are returned as `application/problem+json`.

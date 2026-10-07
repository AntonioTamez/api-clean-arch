# users-crud-api

Objective: .NET 10 Web API, Clean/Hexagonal architecture, `Users` CRUD, in-memory seeded persistence that is swappable for a DB, rich (non-anemic) domain model.

Engram mirror `odd/users-crud-api/tasks`: PENDING (engram MCP failed to connect).

Route: delegated writer (4 layers, 2+ non-trivial files each). Test-first: xUnit, RED observed per layer when feasible.

## Constraints
- Api -> Application -> Domain; Infrastructure -> Application. Domain has zero references.
- Application owns ports (`IUserRepository`); Infrastructure implements them.
- Entities enforce invariants (private setters, factory methods, value objects). No leaking entities through the API (DTOs).
- In-memory repo stores a persistence record, not the entity, so mapping domain <-> persistence is exercised like a real DB.
- Artifacts in English.

## Tasks
- [x] T1 Domain: `User` aggregate, `UserId`, `Email`, `FullName` value objects, domain exceptions + unit tests
- [x] T2 Application: ports, use-case handlers (list/get/create/update/delete), DTOs, errors + unit tests
- [x] T3 Infrastructure: in-memory repository with seed, record mapping, DI
- [x] T4 Api: `UsersController` v1, request contracts, validation, ProblemDetails, composition root + integration tests

## Acceptance
- `dotnet build` and `dotnet test` green.
- GET/POST/PUT/DELETE `/api/v1/users` work against seeded in-memory data.

## Evidence
- `dotnet build` (solution root): 0 warnings, 0 errors.
- `dotnet test` (solution root): Domain.UnitTests 27/27, Application.UnitTests 13/13, Api.IntegrationTests 15/15 passed.
- RED observed for Domain and Application (compile failure before implementation). Api tests written first but implementation followed before a runtime RED run; one real failure found and fixed (`[property:]` validation attributes on records caused 500; moved to parameter targets).
- Run on :5099: `GET /api/v1/users` returned the 5 seeded users; `POST` returned 201 with the created user.
- `rg`: Domain.csproj has no ProjectReference/PackageReference; Application.csproj references only Domain and DI abstractions.

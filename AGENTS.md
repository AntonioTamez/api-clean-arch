# Code Review Rules

Stack: .NET 10, C# 14, ASP.NET Core Web API, Clean Architecture.

## Architecture

- Dependencies point inward only: `Api -> Application -> Domain`; `Infrastructure -> Application`.
- `Domain` has no references to other projects or to framework/infrastructure packages (EF Core, ASP.NET, etc.).
- `Application` defines ports (interfaces) for persistence and external services; `Infrastructure` implements them.
- Controllers/endpoints only map HTTP to use cases. No business logic, no `DbContext` access.
- Use cases live in `Application` (one handler/class per use case). Do not leak entities through the API; use DTOs.
- Organize by feature (screaming architecture), not by technical type only.

## Domain

- Entities enforce their own invariants; avoid public setters and anemic models.
- Prefer value objects and `record` types for immutable concepts.
- Domain errors are explicit (exceptions or `Result<T>`), never magic values or `null`.

## C# Style

- Nullable reference types enabled; no `!` suppression without justification.
- `async` all the way: no `.Result`, `.Wait()`, or `async void`. Pass and honor `CancellationToken`.
- Use constructor injection; no service locator, no static mutable state.
- Use `sealed` by default for classes not designed for inheritance.
- File-scoped namespaces; one public type per file; names in English.
- No commented-out code, no dead code, no unused usings.

## API

- Return correct HTTP status codes and `ProblemDetails` for errors.
- Validate all input at the boundary (FluentValidation or data annotations); never trust request data.
- Never expose stack traces or internal exception messages to clients.
- Version public endpoints; keep contracts backward compatible.

## Data Access

- No raw SQL built by string concatenation; use parameters.
- Use `AsNoTracking()` for read-only queries; avoid N+1 queries.
- Migrations are committed with the change that requires them.

## Security

- No secrets, connection strings, or keys in source; use configuration/user-secrets/environment.
- Protected endpoints declare authorization explicitly; no accidental `[AllowAnonymous]`.
- Do not log sensitive data (tokens, passwords, PII).

## Testing

- New behavior ships with tests; bug fixes ship with a regression test.
- Tests are deterministic: no real clock, network, or shared state; inject `TimeProvider`.
- Domain and Application tests are unit tests with no infrastructure; test Infrastructure/API with integration tests.
- Test names describe behavior, not implementation.

## Response Format

First line must be exactly one of:

- `STATUS: PASSED`
- `STATUS: FAILED`

If FAILED, list each violation as `file:line - rule violated - why`.

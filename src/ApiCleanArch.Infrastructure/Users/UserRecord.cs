namespace ApiCleanArch.Infrastructure.Users;

internal sealed record UserRecord(
    Guid Id,
    string Name,
    string Email,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt);

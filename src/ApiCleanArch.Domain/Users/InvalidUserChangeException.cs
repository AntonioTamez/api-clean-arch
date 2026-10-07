using ApiCleanArch.Domain.Shared;

namespace ApiCleanArch.Domain.Users;

public sealed class InvalidUserChangeException(string message) : DomainException(message);

using ApiCleanArch.Domain.Shared;

namespace ApiCleanArch.Domain.Users;

public sealed class InvalidFullNameException(string message) : DomainException(message);

using ApiCleanArch.Domain.Shared;

namespace ApiCleanArch.Domain.Users;

public sealed class InvalidEmailException(string message) : DomainException(message);

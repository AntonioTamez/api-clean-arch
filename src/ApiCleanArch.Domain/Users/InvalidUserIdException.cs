using ApiCleanArch.Domain.Shared;

namespace ApiCleanArch.Domain.Users;

public sealed class InvalidUserIdException() : DomainException("The user id must not be empty.");

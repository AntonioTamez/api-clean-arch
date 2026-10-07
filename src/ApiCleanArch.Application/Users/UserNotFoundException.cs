using ApiCleanArch.Application.Shared;

namespace ApiCleanArch.Application.Users;

public sealed class UserNotFoundException(Guid id) : AppException($"The user '{id}' was not found.");

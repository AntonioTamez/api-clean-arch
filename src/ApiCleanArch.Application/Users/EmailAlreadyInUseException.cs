using ApiCleanArch.Application.Shared;

namespace ApiCleanArch.Application.Users;

public sealed class EmailAlreadyInUseException(string email) : AppException($"The email '{email}' is already in use.");

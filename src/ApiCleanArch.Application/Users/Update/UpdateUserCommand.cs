namespace ApiCleanArch.Application.Users.Update;

public sealed record UpdateUserCommand(Guid Id, string Name, string Email);

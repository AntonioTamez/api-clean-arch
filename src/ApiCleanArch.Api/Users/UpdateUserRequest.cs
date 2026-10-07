using System.ComponentModel.DataAnnotations;

namespace ApiCleanArch.Api.Users;

public sealed record UpdateUserRequest(
    [Required, StringLength(100, MinimumLength = 2)] string Name,
    [Required, StringLength(254), EmailAddress] string Email);

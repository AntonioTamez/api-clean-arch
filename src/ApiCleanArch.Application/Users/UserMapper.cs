using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Application.Users;

public static class UserMapper
{
    public static UserDto ToDto(this User user) =>
        new(user.Id.Value, user.Name.Value, user.Email.Value, user.CreatedAt, user.UpdatedAt);
}

using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Application.Users.Get;

public sealed class GetUserHandler(IUserRepository repository)
{
    public async Task<UserDto> Handle(GetUserQuery query, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(UserId.Create(query.Id), cancellationToken)
            ?? throw new UserNotFoundException(query.Id);

        return user.ToDto();
    }
}

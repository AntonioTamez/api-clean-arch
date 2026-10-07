namespace ApiCleanArch.Application.Users.List;

public sealed class ListUsersHandler(IUserRepository repository)
{
    public async Task<IReadOnlyList<UserDto>> Handle(ListUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await repository.ListAsync(cancellationToken);
        return [.. users.Select(user => user.ToDto())];
    }
}

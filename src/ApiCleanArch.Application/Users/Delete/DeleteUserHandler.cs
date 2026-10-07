using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Application.Users.Delete;

public sealed class DeleteUserHandler(IUserRepository repository)
{
    public async Task Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var removed = await repository.RemoveAsync(UserId.Create(command.Id), cancellationToken);

        if (!removed)
        {
            throw new UserNotFoundException(command.Id);
        }
    }
}

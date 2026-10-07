using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Application.Users.Update;

public sealed class UpdateUserHandler(IUserRepository repository, TimeProvider timeProvider)
{
    public async Task<UserDto> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var id = UserId.Create(command.Id);
        var user = await repository.GetByIdAsync(id, cancellationToken)
            ?? throw new UserNotFoundException(command.Id);

        var name = FullName.Create(command.Name);
        var email = Email.Create(command.Email);

        if (await repository.ExistsByEmailAsync(email, id, cancellationToken))
        {
            throw new EmailAlreadyInUseException(email.Value);
        }

        var now = timeProvider.GetUtcNow();
        user.Rename(name, now);
        user.ChangeEmail(email, now);
        await repository.UpdateAsync(user, cancellationToken);

        return user.ToDto();
    }
}

using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Application.Users.Create;

public sealed class CreateUserHandler(IUserRepository repository, TimeProvider timeProvider)
{
    public async Task<UserDto> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var name = FullName.Create(command.Name);
        var email = Email.Create(command.Email);

        if (await repository.ExistsByEmailAsync(email, null, cancellationToken))
        {
            throw new EmailAlreadyInUseException(email.Value);
        }

        var user = User.Create(name, email, timeProvider.GetUtcNow());
        await repository.AddAsync(user, cancellationToken);

        return user.ToDto();
    }
}

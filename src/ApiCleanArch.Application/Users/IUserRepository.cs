using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Application.Users;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken);

    Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(Email email, UserId? excluding, CancellationToken cancellationToken);

    Task AddAsync(User user, CancellationToken cancellationToken);

    Task UpdateAsync(User user, CancellationToken cancellationToken);

    Task<bool> RemoveAsync(UserId id, CancellationToken cancellationToken);
}

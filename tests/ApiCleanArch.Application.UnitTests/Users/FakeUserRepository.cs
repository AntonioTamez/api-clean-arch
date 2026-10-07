using ApiCleanArch.Application.Users;
using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Application.UnitTests.Users;

internal sealed class FakeUserRepository : IUserRepository
{
    private readonly Dictionary<UserId, User> _store = [];

    public int Count => _store.Count;

    public Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken) =>
        Task.FromResult(_store.GetValueOrDefault(id));

    public Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<User>>([.. _store.Values]);

    public Task<bool> ExistsByEmailAsync(Email email, UserId? excluding, CancellationToken cancellationToken) =>
        Task.FromResult(_store.Values.Any(u => u.Email == email && u.Id != excluding));

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        _store[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        _store[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(UserId id, CancellationToken cancellationToken) =>
        Task.FromResult(_store.Remove(id));
}

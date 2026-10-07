using System.Collections.Concurrent;
using ApiCleanArch.Application.Users;
using ApiCleanArch.Domain.Users;

namespace ApiCleanArch.Infrastructure.Users;

internal sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, UserRecord> _store;

    public InMemoryUserRepository(IEnumerable<UserRecord> seed) =>
        _store = new ConcurrentDictionary<Guid, UserRecord>(seed.ToDictionary(record => record.Id));

    public Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = _store.TryGetValue(id.Value, out var record) ? record.ToDomain() : null;
        return Task.FromResult(user);
    }

    public Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<User> users =
        [
            .. _store.Values.OrderBy(record => record.CreatedAt).ThenBy(record => record.Id).Select(record => record.ToDomain()),
        ];
        return Task.FromResult(users);
    }

    public Task<bool> ExistsByEmailAsync(Email email, UserId? excluding, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var exists = _store.Values.Any(record =>
            record.Email == email.Value && record.Id != excluding?.Value);
        return Task.FromResult(exists);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _store[user.Id.Value] = user.ToRecord();
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _store[user.Id.Value] = user.ToRecord();
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(UserId id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_store.TryRemove(id.Value, out _));
    }
}

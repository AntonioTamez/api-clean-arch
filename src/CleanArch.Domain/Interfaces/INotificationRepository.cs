using CleanArch.Domain.Entities;

namespace CleanArch.Domain.Interfaces;

public interface INotificationRepository : IRepository<Notification>
{
    Task<IReadOnlyList<Notification>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetUnreadByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Notification>> GetRecentAsync(int count, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadForUserAsync(string userId, CancellationToken cancellationToken = default);
}

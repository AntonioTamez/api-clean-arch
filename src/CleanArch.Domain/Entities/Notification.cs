using CleanArch.Domain.Common;

namespace CleanArch.Domain.Entities;

/// <summary>
/// Entidad de notificación para sistema de notificaciones en tiempo real
/// </summary>
public class Notification : BaseAuditableEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public NotificationType Type { get; private set; }
    public string? UserId { get; private set; }
    public string? EntityType { get; private set; }
    public Guid? EntityId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Notification() { } // Para EF Core

    private Notification(string title, string message, NotificationType type, string? userId, string? entityType, Guid? entityId)
    {
        Title = title;
        Message = message;
        Type = type;
        UserId = userId;
        EntityType = entityType;
        EntityId = entityId;
        IsRead = false;
    }

    public static Notification Create(string title, string message, NotificationType type, string? userId = null, string? entityType = null, Guid? entityId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required", nameof(title));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message is required", nameof(message));

        return new Notification(title, message, type, userId, entityType, entityId);
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }

    public void MarkAsUnread()
    {
        IsRead = false;
        ReadAt = null;
    }
}

/// <summary>
/// Tipos de notificación
/// </summary>
public enum NotificationType
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
    ProjectCreated = 10,
    ProjectUpdated = 11,
    ProjectCompleted = 12,
    CapabilityCreated = 20,
    CapabilityUpdated = 21,
    BusinessRuleCreated = 30,
    BusinessRuleActivated = 31,
    BusinessRuleDeactivated = 32,
    WikiPageCreated = 40,
    WikiPagePublished = 41,
    WikiPageUpdated = 42
}

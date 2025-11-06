using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Notifications.DTOs;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;

namespace CleanArch.Infrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly IRealtimeMessenger _messenger;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(
        IRealtimeMessenger messenger,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _messenger = messenger;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task SendToAllAsync(string title, string message, string type = "Info")
    {
        await _messenger.SendToAllAsync("ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Type = type,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task SendToUserAsync(string userId, string title, string message, string type = "Info")
    {
        await _messenger.SendToUserAsync(userId, "ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Type = type,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task SendToGroupAsync(string groupName, string title, string message, string type = "Info")
    {
        await _messenger.SendToGroupAsync(groupName, "ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Type = type,
            Timestamp = DateTime.UtcNow
        });
    }

    public async Task<NotificationDto> CreateAndSendAsync(SendNotificationDto dto)
    {
        // Parsear tipo de notificación
        if (!Enum.TryParse<NotificationType>(dto.Type, true, out var notificationType))
        {
            notificationType = NotificationType.Info;
        }

        // Crear notificación en BD
        var notification = Notification.Create(
            dto.Title,
            dto.Message,
            notificationType,
            dto.UserId,
            dto.EntityType,
            dto.EntityId);

        await _notificationRepository.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        // Enviar por SignalR
        if (string.IsNullOrEmpty(dto.UserId))
        {
            await SendToAllAsync(dto.Title, dto.Message, dto.Type);
        }
        else
        {
            await SendToUserAsync(dto.UserId, dto.Title, dto.Message, dto.Type);
        }

        // Retornar DTO
        return new NotificationDto
        {
            Id = notification.Id,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type.ToString(),
            UserId = notification.UserId,
            EntityType = notification.EntityType,
            EntityId = notification.EntityId,
            IsRead = notification.IsRead,
            ReadAt = notification.ReadAt,
            CreatedAt = notification.CreatedAt
        };
    }
}

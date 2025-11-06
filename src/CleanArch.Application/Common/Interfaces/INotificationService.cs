using CleanArch.Application.Notifications.DTOs;

namespace CleanArch.Application.Common.Interfaces;

/// <summary>
/// Servicio para enviar notificaciones en tiempo real
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Envía notificación a todos los usuarios conectados
    /// </summary>
    Task SendToAllAsync(string title, string message, string type = "Info");

    /// <summary>
    /// Envía notificación a un usuario específico
    /// </summary>
    Task SendToUserAsync(string userId, string title, string message, string type = "Info");

    /// <summary>
    /// Envía notificación a un grupo específico
    /// </summary>
    Task SendToGroupAsync(string groupName, string title, string message, string type = "Info");

    /// <summary>
    /// Persiste y envía notificación
    /// </summary>
    Task<NotificationDto> CreateAndSendAsync(SendNotificationDto notification);
}

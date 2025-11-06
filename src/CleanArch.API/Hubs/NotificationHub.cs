using CleanArch.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CleanArch.API.Hubs;

/// <summary>
/// Hub de SignalR para notificaciones en tiempo real
/// </summary>
[Authorize]
public class NotificationHub : Hub, INotificationHub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = Context.User?.Identity?.Name;

        if (!string.IsNullOrEmpty(userId))
        {
            // Agregar conexión al grupo del usuario
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        // Notificar que el usuario se conectó
        await Clients.All.SendAsync("UserConnected", new
        {
            ConnectionId = Context.ConnectionId,
            Username = username,
            ConnectedAt = DateTime.UtcNow
        });

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var username = Context.User?.Identity?.Name;

        if (!string.IsNullOrEmpty(userId))
        {
            // Remover del grupo del usuario
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }

        // Notificar que el usuario se desconectó
        await Clients.All.SendAsync("UserDisconnected", new
        {
            ConnectionId = Context.ConnectionId,
            Username = username,
            DisconnectedAt = DateTime.UtcNow
        });

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Cliente solicita unirse a un grupo específico (ej: proyecto, capacidad)
    /// </summary>
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("JoinedGroup", groupName);
    }

    /// <summary>
    /// Cliente solicita salir de un grupo
    /// </summary>
    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Caller.SendAsync("LeftGroup", groupName);
    }

    /// <summary>
    /// Cliente envía notificación a todos
    /// </summary>
    public async Task SendNotificationToAll(string title, string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Type = "Info",
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Cliente envía notificación a un usuario específico
    /// </summary>
    public async Task SendNotificationToUser(string userId, string title, string message)
    {
        await Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", new
        {
            Title = title,
            Message = message,
            Type = "Info",
            Timestamp = DateTime.UtcNow
        });
    }
}

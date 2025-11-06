namespace CleanArch.Application.Common.Interfaces;

/// <summary>
/// Abstracción para enviar mensajes en tiempo real (desacoplado de SignalR)
/// </summary>
public interface IRealtimeMessenger
{
    Task SendToAllAsync(string method, object data);
    Task SendToUserAsync(string userId, string method, object data);
    Task SendToGroupAsync(string groupName, string method, object data);
}

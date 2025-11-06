using CleanArch.API.Hubs;
using CleanArch.Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace CleanArch.API.Services;

/// <summary>
/// Implementación de IRealtimeMessenger usando SignalR
/// </summary>
public class SignalRMessenger : IRealtimeMessenger
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRMessenger(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendToAllAsync(string method, object data)
    {
        await _hubContext.Clients.All.SendAsync(method, data);
    }

    public async Task SendToUserAsync(string userId, string method, object data)
    {
        await _hubContext.Clients.Group($"user_{userId}").SendAsync(method, data);
    }

    public async Task SendToGroupAsync(string groupName, string method, object data)
    {
        await _hubContext.Clients.Group(groupName).SendAsync(method, data);
    }
}

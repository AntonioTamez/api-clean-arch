using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Notifications.DTOs;
using CleanArch.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

/// <summary>
/// Controller para gestión de notificaciones
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationsController(
        INotificationService notificationService,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _notificationService = notificationService;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Obtiene todas las notificaciones del usuario actual
    /// </summary>
    [HttpGet("my-notifications")]
    [ProducesResponseType(typeof(List<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var notifications = await _notificationRepository.GetByUserIdAsync(userId);

        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type.ToString(),
            UserId = n.UserId,
            EntityType = n.EntityType,
            EntityId = n.EntityId,
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Obtiene notificaciones no leídas del usuario actual
    /// </summary>
    [HttpGet("unread")]
    [ProducesResponseType(typeof(List<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadNotifications()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);

        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type.ToString(),
            UserId = n.UserId,
            EntityType = n.EntityType,
            EntityId = n.EntityId,
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt
        }).ToList();

        return Ok(dtos);
    }

    /// <summary>
    /// Obtiene el conteo de notificaciones no leídas
    /// </summary>
    [HttpGet("unread/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnreadCount()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var count = await _notificationRepository.GetUnreadCountByUserIdAsync(userId);
        return Ok(count);
    }

    /// <summary>
    /// Marca una notificación como leída
    /// </summary>
    [HttpPut("{id}/mark-as-read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var notification = await _notificationRepository.GetByIdAsync(id);
        if (notification == null)
            return NotFound();

        notification.MarkAsRead();
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Marca todas las notificaciones del usuario como leídas
    /// </summary>
    [HttpPut("mark-all-as-read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        await _notificationRepository.MarkAllAsReadForUserAsync(userId);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Envía una notificación manual [Admin only]
    /// </summary>
    [HttpPost("send")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationDto dto)
    {
        var notification = await _notificationService.CreateAndSendAsync(dto);
        return CreatedAtAction(nameof(GetMyNotifications), null, notification);
    }

    /// <summary>
    /// Obtiene notificaciones recientes (todas) [Admin only]
    /// </summary>
    [HttpGet("recent")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(List<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentNotifications([FromQuery] int count = 50)
    {
        var notifications = await _notificationRepository.GetRecentAsync(count);

        var dtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type.ToString(),
            UserId = n.UserId,
            EntityType = n.EntityType,
            EntityId = n.EntityId,
            IsRead = n.IsRead,
            ReadAt = n.ReadAt,
            CreatedAt = n.CreatedAt
        }).ToList();

        return Ok(dtos);
    }
}

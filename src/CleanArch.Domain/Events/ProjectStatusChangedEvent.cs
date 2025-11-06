using CleanArch.Domain.Common;
using CleanArch.Domain.Enums;

namespace CleanArch.Domain.Events;

/// <summary>
/// Evento de dominio que se dispara cuando cambia el estado de un proyecto
/// </summary>
public record ProjectStatusChangedEvent(
    Guid ProjectId,
    ProjectStatus OldStatus,
    ProjectStatus NewStatus) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

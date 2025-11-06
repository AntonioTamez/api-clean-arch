using CleanArch.Domain.Common;
using CleanArch.Domain.Enums;

namespace CleanArch.Domain.Events;

/// <summary>
/// Evento que se dispara cuando cambia el estado de una regla de negocio
/// </summary>
public record BusinessRuleStatusChangedEvent(
    Guid BusinessRuleId,
    BusinessRuleStatus OldStatus,
    BusinessRuleStatus NewStatus) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

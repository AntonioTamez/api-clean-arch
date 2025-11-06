using CleanArch.Domain.Common;

namespace CleanArch.Domain.Events;

/// <summary>
/// Evento que se dispara cuando se crea una nueva regla de negocio
/// </summary>
public record BusinessRuleCreatedEvent(
    Guid BusinessRuleId,
    Guid CapabilityId,
    string RuleCode,
    string RuleName) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

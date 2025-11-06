using CleanArch.Domain.Common;

namespace CleanArch.Domain.Events;

/// <summary>
/// Evento que se dispara cuando se crea una nueva capacidad
/// </summary>
public record CapabilityCreatedEvent(
    Guid CapabilityId,
    Guid ApplicationId,
    string CapabilityName) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

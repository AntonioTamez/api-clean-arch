using CleanArch.Domain.Common;

namespace CleanArch.Domain.Events;

/// <summary>
/// Evento de dominio que se dispara cuando se crea un nuevo proyecto
/// </summary>
public record ProjectCreatedEvent(
    Guid ProjectId,
    string ProjectCode,
    string ProjectName) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

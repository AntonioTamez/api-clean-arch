using CleanArch.Domain.Common;

namespace CleanArch.Domain.Events;

/// <summary>
/// Evento de dominio que se dispara cuando se añade una aplicación a un proyecto
/// </summary>
public record ApplicationAddedToProjectEvent(
    Guid ProjectId,
    Guid ApplicationId,
    string ApplicationName) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

using CleanArch.Domain.Common;

namespace CleanArch.Domain.Events;

/// <summary>
/// Evento que se dispara cuando se crea una nueva versión de una página wiki
/// </summary>
public record WikiPageVersionCreatedEvent(
    Guid WikiPageId,
    int VersionNumber,
    string AuthorId) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

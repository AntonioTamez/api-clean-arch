using CleanArch.Domain.Common;

namespace CleanArch.Domain.Events;

/// <summary>
/// Evento que se dispara cuando se crea una nueva página wiki
/// </summary>
public record WikiPageCreatedEvent(
    Guid WikiPageId,
    string Title,
    string Slug) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

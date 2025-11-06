using CleanArch.Domain.Common;

namespace CleanArch.Domain.Events;

/// <summary>
/// Evento que se dispara cuando se publica una página wiki
/// </summary>
public record WikiPagePublishedEvent(
    Guid WikiPageId,
    string Title) : IDomainEvent
{
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
}

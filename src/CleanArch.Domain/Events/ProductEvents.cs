using CleanArch.Domain.Common;
using CleanArch.Domain.ValueObjects;

namespace CleanArch.Domain.Events;

/// <summary>
/// Evento: Producto creado
/// </summary>
public record ProductCreatedEvent(Guid ProductId, string ProductName) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

/// <summary>
/// Evento: Precio del producto cambiado
/// </summary>
public record ProductPriceChangedEvent(Guid ProductId, Money OldPrice, Money NewPrice) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

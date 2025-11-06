using CleanArch.Domain.Common;
using CleanArch.Domain.Events;
using CleanArch.Domain.Exceptions;
using CleanArch.Domain.ValueObjects;

namespace CleanArch.Domain.Entities;

/// <summary>
/// Entidad Product del dominio
/// </summary>
public class Product : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;
    public int Stock { get; private set; }
    public bool IsActive { get; private set; }

    // Constructor privado para EF Core
    private Product()
    {
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    public static Product Create(string name, string description, Money price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required");

        if (price.Amount <= 0)
            throw new DomainException("Product price must be greater than zero");

        if (stock < 0)
            throw new DomainException("Stock cannot be negative");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description ?? string.Empty,
            Price = price,
            Stock = stock,
            IsActive = true
        };

        // Agregar evento del dominio
        product.AddDomainEvent(new ProductCreatedEvent(product.Id, product.Name));

        return product;
    }

    /// <summary>
    /// Actualiza el precio del producto
    /// </summary>
    public void UpdatePrice(Money newPrice)
    {
        if (newPrice.Amount <= 0)
            throw new DomainException("Price must be greater than zero");

        if (newPrice.Currency != Price.Currency)
            throw new DomainException("Cannot change currency");

        var oldPrice = Price;
        Price = newPrice;

        AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, newPrice));
    }

    /// <summary>
    /// Actualiza el stock del producto
    /// </summary>
    public void UpdateStock(int newStock)
    {
        if (newStock < 0)
            throw new DomainException("Stock cannot be negative");

        Stock = newStock;
    }

    /// <summary>
    /// Reduce el stock del producto
    /// </summary>
    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        if (Stock < quantity)
            throw new DomainException($"Insufficient stock. Available: {Stock}, Requested: {quantity}");

        Stock -= quantity;
    }

    /// <summary>
    /// Incrementa el stock del producto
    /// </summary>
    public void AddStock(int quantity)
    {
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        Stock += quantity;
    }

    /// <summary>
    /// Actualiza la información del producto
    /// </summary>
    public void UpdateInfo(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name is required");

        Name = name;
        Description = description ?? string.Empty;
    }

    /// <summary>
    /// Activa el producto
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Desactiva el producto
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }
}

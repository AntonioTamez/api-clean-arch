using CleanArch.Application.Common.Models;
using MediatR;

namespace CleanArch.Application.Products.Commands.CreateProduct;

/// <summary>
/// Command para crear un producto
/// </summary>
public record CreateProductCommand : IRequest<Result<Guid>>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = "USD";
    public int Stock { get; init; }
}

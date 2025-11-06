using CleanArch.Application.Common.Models;
using CleanArch.Application.Products.DTOs;
using MediatR;

namespace CleanArch.Application.Products.Queries.GetProducts;

/// <summary>
/// Query para obtener lista de productos
/// </summary>
public record GetProductsQuery : IRequest<Result<List<ProductDto>>>
{
    public bool OnlyActive { get; init; } = false;
}

using CleanArch.Application.Common.Models;
using CleanArch.Application.Products.DTOs;
using MediatR;

namespace CleanArch.Application.Products.Queries.GetProduct;

/// <summary>
/// Query para obtener un producto por ID
/// </summary>
public record GetProductQuery(Guid Id) : IRequest<Result<ProductDto>>;

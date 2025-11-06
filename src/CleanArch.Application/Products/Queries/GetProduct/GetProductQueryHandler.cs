using AutoMapper;
using CleanArch.Application.Common.Models;
using CleanArch.Application.Products.DTOs;
using CleanArch.Domain.Interfaces;
using MediatR;

namespace CleanArch.Application.Products.Queries.GetProduct;

/// <summary>
/// Handler para GetProductQuery
/// </summary>
public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<ProductDto>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductQueryHandler(
        IProductRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id, cancellationToken);

        if (product is null)
        {
            return Result.Failure<ProductDto>(
                new Error("Product.NotFound", $"Product with ID {request.Id} was not found"));
        }

        var productDto = _mapper.Map<ProductDto>(product);

        return Result.Success(productDto);
    }
}

using AutoMapper;
using CleanArch.Application.Common.Models;
using CleanArch.Application.Products.DTOs;
using CleanArch.Domain.Interfaces;
using MediatR;

namespace CleanArch.Application.Products.Queries.GetProducts;

/// <summary>
/// Handler para GetProductsQuery
/// </summary>
public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<List<ProductDto>>>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(
        IProductRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<List<ProductDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var products = request.OnlyActive
                ? await _repository.GetActiveProductsAsync(cancellationToken)
                : await _repository.GetAllAsync(cancellationToken);

            var productDtos = _mapper.Map<List<ProductDto>>(products);

            return Result.Success(productDtos);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<ProductDto>>(
                new Error("GetProducts.Error", ex.Message));
        }
    }
}

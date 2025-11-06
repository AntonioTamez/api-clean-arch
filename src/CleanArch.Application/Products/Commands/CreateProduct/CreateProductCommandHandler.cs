using CleanArch.Application.Common.Models;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;
using CleanArch.Domain.ValueObjects;
using MediatR;

namespace CleanArch.Application.Products.Commands.CreateProduct;

/// <summary>
/// Handler para CreateProductCommand
/// </summary>
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // Crear Value Object Money
            var price = Money.Create(request.Price, request.Currency);

            // Crear entidad Product
            var product = Product.Create(
                request.Name,
                request.Description,
                price,
                request.Stock);

            // Agregar al repositorio
            await _repository.AddAsync(product, cancellationToken);

            // Guardar cambios
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(product.Id);
        }
        catch (Exception ex)
        {
            return Result.Failure<Guid>(new Error("CreateProduct.Error", ex.Message));
        }
    }
}

using CleanArch.Domain.Entities;

namespace CleanArch.Domain.Interfaces;

/// <summary>
/// Interfaz específica del repositorio de productos
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}

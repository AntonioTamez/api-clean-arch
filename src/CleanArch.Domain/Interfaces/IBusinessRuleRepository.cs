using CleanArch.Domain.Entities;

namespace CleanArch.Domain.Interfaces;

/// <summary>
/// Repositorio para la entidad BusinessRule
/// </summary>
public interface IBusinessRuleRepository : IRepository<BusinessRule>
{
    Task<BusinessRule?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<BusinessRule>> GetByCapabilityIdAsync(Guid capabilityId, CancellationToken cancellationToken = default);
    Task<List<BusinessRule>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

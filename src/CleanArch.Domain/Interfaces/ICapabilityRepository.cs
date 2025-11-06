using CleanArch.Domain.Entities;

namespace CleanArch.Domain.Interfaces;

/// <summary>
/// Repositorio para la entidad Capability
/// </summary>
public interface ICapabilityRepository : IRepository<Capability>
{
    Task<List<Capability>> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken = default);
    Task<Capability?> GetWithBusinessRulesAsync(Guid id, CancellationToken cancellationToken = default);
}

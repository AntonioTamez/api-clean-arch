using CleanArch.Domain.Entities;

namespace CleanArch.Domain.Interfaces;

/// <summary>
/// Repositorio para la entidad Project
/// </summary>
public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<List<Project>> GetAllWithApplicationsAsync(CancellationToken cancellationToken = default);
}

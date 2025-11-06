using CleanArch.Domain.Entities;
using CleanArch.Domain.Enums;

namespace CleanArch.Domain.Interfaces;

/// <summary>
/// Repositorio para la entidad WikiPage
/// </summary>
public interface IWikiPageRepository : IRepository<WikiPage>
{
    Task<WikiPage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<WikiPage?> GetWithVersionsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<WikiPage>> GetByEntityAsync(WikiEntityType entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task<List<WikiPage>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<WikiPage>> GetPublishedAsync(CancellationToken cancellationToken = default);
}

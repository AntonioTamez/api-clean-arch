using CleanArch.Domain.Entities;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Persistence.Repositories;

public class WikiPageRepository : IWikiPageRepository
{
    private readonly ApplicationDbContext _context;

    public WikiPageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WikiPage?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WikiPages
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<WikiPage?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _context.WikiPages
            .FirstOrDefaultAsync(w => w.Slug.Value == slug, cancellationToken);
    }

    public async Task<WikiPage?> GetWithVersionsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WikiPages
            .Include(w => w.Versions.OrderByDescending(v => v.VersionNumber))
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<WikiPage>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WikiPages
            .OrderByDescending(w => w.ModifiedAt ?? w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WikiPage>> GetByEntityAsync(WikiEntityType entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await _context.WikiPages
            .Where(w => w.EntityType == entityType && w.EntityId == entityId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WikiPage>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var lowerSearchTerm = searchTerm.ToLower();

        return await _context.WikiPages
            .Where(w => 
                w.Title.ToLower().Contains(lowerSearchTerm) ||
                w.CurrentContent.ToLower().Contains(lowerSearchTerm) ||
                w.Category.ToLower().Contains(lowerSearchTerm))
            .OrderByDescending(w => w.ViewCount)
            .ThenByDescending(w => w.ModifiedAt ?? w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<WikiPage>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WikiPages
            .Where(w => w.IsPublished)
            .OrderByDescending(w => w.ModifiedAt ?? w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<WikiPage> AddAsync(WikiPage wikiPage, CancellationToken cancellationToken = default)
    {
        await _context.WikiPages.AddAsync(wikiPage, cancellationToken);
        return wikiPage;
    }

    public Task UpdateAsync(WikiPage wikiPage, CancellationToken cancellationToken = default)
    {
        _context.WikiPages.Update(wikiPage);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(WikiPage wikiPage, CancellationToken cancellationToken = default)
    {
        _context.WikiPages.Remove(wikiPage);
        return Task.CompletedTask;
    }
}

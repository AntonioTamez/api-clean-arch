using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Persistence.Repositories;

public class CapabilityRepository : ICapabilityRepository
{
    private readonly ApplicationDbContext _context;

    public CapabilityRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Capability?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Capabilities
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Capability>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Capabilities
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Capability>> GetByApplicationIdAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        return await _context.Capabilities
            .Where(c => c.ApplicationId == applicationId)
            .OrderBy(c => c.Priority)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Capability?> GetWithBusinessRulesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Capabilities
            .Include(c => c.BusinessRules)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Capability> AddAsync(Capability capability, CancellationToken cancellationToken = default)
    {
        await _context.Capabilities.AddAsync(capability, cancellationToken);
        return capability;
    }

    public Task UpdateAsync(Capability capability, CancellationToken cancellationToken = default)
    {
        _context.Capabilities.Update(capability);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Capability capability, CancellationToken cancellationToken = default)
    {
        _context.Capabilities.Remove(capability);
        return Task.CompletedTask;
    }
}

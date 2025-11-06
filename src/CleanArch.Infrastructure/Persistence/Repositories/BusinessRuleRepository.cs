using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Infrastructure.Persistence.Repositories;

public class BusinessRuleRepository : IBusinessRuleRepository
{
    private readonly ApplicationDbContext _context;

    public BusinessRuleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BusinessRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessRules
            .FirstOrDefaultAsync(br => br.Id == id, cancellationToken);
    }

    public async Task<BusinessRule?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessRules
            .FirstOrDefaultAsync(br => br.Code.Value == code, cancellationToken);
    }

    public async Task<IReadOnlyList<BusinessRule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BusinessRules
            .OrderByDescending(br => br.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BusinessRule>> GetByCapabilityIdAsync(Guid capabilityId, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessRules
            .Where(br => br.CapabilityId == capabilityId)
            .OrderBy(br => br.Priority)
            .ThenBy(br => br.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<BusinessRule>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var lowerSearchTerm = searchTerm.ToLower();

        return await _context.BusinessRules
            .Where(br => 
                br.Name.ToLower().Contains(lowerSearchTerm) ||
                br.Description.ToLower().Contains(lowerSearchTerm) ||
                br.Code.Value.ToLower().Contains(lowerSearchTerm))
            .OrderBy(br => br.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<BusinessRule> AddAsync(BusinessRule businessRule, CancellationToken cancellationToken = default)
    {
        await _context.BusinessRules.AddAsync(businessRule, cancellationToken);
        return businessRule;
    }

    public Task UpdateAsync(BusinessRule businessRule, CancellationToken cancellationToken = default)
    {
        _context.BusinessRules.Update(businessRule);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(BusinessRule businessRule, CancellationToken cancellationToken = default)
    {
        _context.BusinessRules.Remove(businessRule);
        return Task.CompletedTask;
    }
}

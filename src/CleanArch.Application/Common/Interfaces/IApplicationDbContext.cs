using CleanArch.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Application.Common.Interfaces;

/// <summary>
/// Interfaz del DbContext de la aplicación
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Project> Projects { get; }
    DbSet<Domain.Entities.Application> Applications { get; }
    DbSet<Capability> Capabilities { get; }
    DbSet<BusinessRule> BusinessRules { get; }
    DbSet<WikiPage> WikiPages { get; }
    DbSet<WikiPageVersion> WikiPageVersions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

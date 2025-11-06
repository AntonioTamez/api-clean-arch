using CleanArch.Application.Common.Interfaces;
using CleanArch.Domain.Interfaces;
using CleanArch.Infrastructure.Auth;
using CleanArch.Infrastructure.Export;
using CleanArch.Infrastructure.Persistence;
using CleanArch.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure;

/// <summary>
/// Configuración de Dependency Injection para la capa Infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Registrar DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Registrar IApplicationDbContext
        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Registrar IUnitOfWork
        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        // Registrar Repositories
        services.AddScoped<IProductRepository, ProductRepository>(); // TODO: Eliminar cuando se retire Product
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ICapabilityRepository, CapabilityRepository>();
        services.AddScoped<IBusinessRuleRepository, BusinessRuleRepository>();
        services.AddScoped<IWikiPageRepository, WikiPageRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        // Registrar servicios de autenticación
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Registrar servicios de exportación
        services.AddScoped<IExcelExportService, ExcelExportService>();

        return services;
    }
}

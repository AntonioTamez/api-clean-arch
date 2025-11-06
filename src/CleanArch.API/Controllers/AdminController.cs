using CleanArch.Infrastructure.Persistence;
using CleanArch.Infrastructure.Persistence.Seeders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.API.Controllers;

/// <summary>
/// Controller para operaciones administrativas del sistema
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AdminController> _logger;
    private readonly ILogger<DatabaseSeeder> _seederLogger;

    public AdminController(
        ApplicationDbContext context,
        ILogger<AdminController> logger,
        ILogger<DatabaseSeeder> seederLogger)
    {
        _context = context;
        _logger = logger;
        _seederLogger = seederLogger;
    }

    /// <summary>
    /// Obtiene información general de la base de datos
    /// </summary>
    [HttpGet("database/info")]
    [ProducesResponseType(typeof(DatabaseInfo), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDatabaseInfo()
    {
        var info = new DatabaseInfo
        {
            DatabaseName = _context.Database.GetDbConnection().Database,
            CanConnect = await _context.Database.CanConnectAsync(),
            PendingMigrations = (await _context.Database.GetPendingMigrationsAsync()).ToList(),
            AppliedMigrations = (await _context.Database.GetAppliedMigrationsAsync()).ToList(),
            
            // Contadores de registros
            ProjectsCount = await _context.Projects.CountAsync(),
            ApplicationsCount = await _context.Applications.CountAsync(),
            CapabilitiesCount = await _context.Capabilities.CountAsync(),
            BusinessRulesCount = await _context.BusinessRules.CountAsync(),
            WikiPagesCount = await _context.WikiPages.CountAsync(),
            UsersCount = await _context.Users.CountAsync(),
            NotificationsCount = await _context.Notifications.CountAsync()
        };

        return Ok(info);
    }

    /// <summary>
    /// Ejecuta los seeders manualmente (solo si la BD está vacía)
    /// </summary>
    [HttpPost("database/seed")]
    [ProducesResponseType(typeof(SeedResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SeedDatabase()
    {
        try
        {
            _logger.LogInformation("Manual seed requested by admin");

            var seeder = new DatabaseSeeder(_context, _seederLogger);
            await seeder.SeedAsync();

            var result = new SeedResult
            {
                Success = true,
                Message = "Database seeded successfully",
                ProjectsSeeded = await _context.Projects.CountAsync(),
                ApplicationsSeeded = await _context.Applications.CountAsync(),
                CapabilitiesSeeded = await _context.Capabilities.CountAsync(),
                BusinessRulesSeeded = await _context.BusinessRules.CountAsync(),
                WikiPagesSeeded = await _context.WikiPages.CountAsync(),
                UsersSeeded = await _context.Users.CountAsync(),
                NotificationsSeeded = await _context.Notifications.CountAsync()
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during manual seed");
            return BadRequest(new SeedResult
            {
                Success = false,
                Message = $"Seed failed: {ex.Message}"
            });
        }
    }

    /// <summary>
    /// Aplica migraciones pendientes
    /// </summary>
    [HttpPost("database/migrate")]
    [ProducesResponseType(typeof(MigrationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MigrateDatabase()
    {
        try
        {
            _logger.LogInformation("Manual migration requested by admin");

            var pendingMigrations = (await _context.Database.GetPendingMigrationsAsync()).ToList();
            
            if (!pendingMigrations.Any())
            {
                return Ok(new MigrationResult
                {
                    Success = true,
                    Message = "Database is already up to date",
                    MigrationsApplied = new List<string>()
                });
            }

            await _context.Database.MigrateAsync();

            var result = new MigrationResult
            {
                Success = true,
                Message = $"Applied {pendingMigrations.Count} migration(s)",
                MigrationsApplied = pendingMigrations
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during manual migration");
            return BadRequest(new MigrationResult
            {
                Success = false,
                Message = $"Migration failed: {ex.Message}",
                MigrationsApplied = new List<string>()
            });
        }
    }

    /// <summary>
    /// Limpia todos los datos de la base de datos (PELIGROSO - solo desarrollo)
    /// </summary>
    [HttpDelete("database/clear")]
    [ProducesResponseType(typeof(ClearResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ClearDatabase([FromQuery] string confirmation)
    {
        // Requerir confirmación explícita
        if (confirmation != "CONFIRM_DELETE_ALL_DATA")
        {
            return BadRequest(new
            {
                Message = "Para confirmar, envía el parámetro confirmation=CONFIRM_DELETE_ALL_DATA"
            });
        }

        // Solo permitir en desarrollo
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment != "Development")
        {
            return StatusCode(403, new
            {
                Message = "Esta operación solo está disponible en ambiente de desarrollo"
            });
        }

        try
        {
            _logger.LogWarning("Database clear requested by admin");

            // Contar registros antes
            var beforeCounts = new
            {
                Projects = await _context.Projects.CountAsync(),
                Applications = await _context.Applications.CountAsync(),
                Capabilities = await _context.Capabilities.CountAsync(),
                BusinessRules = await _context.BusinessRules.CountAsync(),
                WikiPages = await _context.WikiPages.CountAsync(),
                Users = await _context.Users.CountAsync(),
                Notifications = await _context.Notifications.CountAsync()
            };

            // Eliminar en orden inverso a las relaciones
            _context.Notifications.RemoveRange(_context.Notifications);
            _context.WikiPageVersions.RemoveRange(_context.WikiPageVersions);
            _context.WikiPages.RemoveRange(_context.WikiPages);
            _context.BusinessRules.RemoveRange(_context.BusinessRules);
            _context.Capabilities.RemoveRange(_context.Capabilities);
            _context.Applications.RemoveRange(_context.Applications);
            _context.Projects.RemoveRange(_context.Projects);
            _context.Users.RemoveRange(_context.Users);

            await _context.SaveChangesAsync();

            var result = new ClearResult
            {
                Success = true,
                Message = "All data cleared successfully",
                RecordsDeleted = beforeCounts.Projects + beforeCounts.Applications + 
                                beforeCounts.Capabilities + beforeCounts.BusinessRules + 
                                beforeCounts.WikiPages + beforeCounts.Users + beforeCounts.Notifications
            };

            _logger.LogWarning($"Database cleared: {result.RecordsDeleted} records deleted");

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during database clear");
            return BadRequest(new ClearResult
            {
                Success = false,
                Message = $"Clear failed: {ex.Message}",
                RecordsDeleted = 0
            });
        }
    }
}

#region DTOs

public class DatabaseInfo
{
    public string DatabaseName { get; set; } = string.Empty;
    public bool CanConnect { get; set; }
    public List<string> PendingMigrations { get; set; } = new();
    public List<string> AppliedMigrations { get; set; } = new();
    public int ProjectsCount { get; set; }
    public int ApplicationsCount { get; set; }
    public int CapabilitiesCount { get; set; }
    public int BusinessRulesCount { get; set; }
    public int WikiPagesCount { get; set; }
    public int UsersCount { get; set; }
    public int NotificationsCount { get; set; }
}

public class SeedResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int ProjectsSeeded { get; set; }
    public int ApplicationsSeeded { get; set; }
    public int CapabilitiesSeeded { get; set; }
    public int BusinessRulesSeeded { get; set; }
    public int WikiPagesSeeded { get; set; }
    public int UsersSeeded { get; set; }
    public int NotificationsSeeded { get; set; }
}

public class MigrationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> MigrationsApplied { get; set; } = new();
}

public class ClearResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int RecordsDeleted { get; set; }
}

#endregion

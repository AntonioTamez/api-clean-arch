using CleanArch.Domain.Entities;
using CleanArch.Domain.Enums;
using CleanArch.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CleanArch.Infrastructure.Persistence.Seeders;

/// <summary>
/// Seeder principal para inicializar la base de datos con datos de prueba
/// </summary>
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta todos los seeders en orden
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("Starting database seeding...");

            // Verificar si ya hay datos
            if (await _context.Projects.AnyAsync())
            {
                _logger.LogInformation("Database already seeded. Skipping...");
                return;
            }

            // Ejecutar seeders en orden (respetando relaciones)
            var users = await SeedUsersAsync();
            var projects = await SeedProjectsAsync();
            var applications = await SeedApplicationsAsync(projects);
            var capabilities = await SeedCapabilitiesAsync(applications);
            var businessRules = await SeedBusinessRulesAsync(capabilities);
            var wikiPages = await SeedWikiPagesAsync(users);
            await SeedNotificationsAsync(users);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task<List<User>> SeedUsersAsync()
    {
        _logger.LogInformation("Seeding Users...");

        // Password: Admin123! (hash con BCrypt)
        var adminHash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        var userHash = BCrypt.Net.BCrypt.HashPassword("User123!");

        var users = new List<User>
        {
            User.Create("admin", "admin@cleanarch.com", adminHash, "Administrator"),
            User.Create("jdoe", "john.doe@cleanarch.com", userHash, "John Doe"),
            User.Create("mjohnson", "mary.johnson@cleanarch.com", userHash, "Mary Johnson"),
            User.Create("rsmith", "robert.smith@cleanarch.com", userHash, "Robert Smith")
        };

        // Asignar roles
        users[0].AddRole("Admin");
        users[0].AddRole("User");
        users[1].AddRole("User");
        users[2].AddRole("User");
        users[3].AddRole("User");

        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {users.Count} users");
        return users;
    }

    private async Task<List<Project>> SeedProjectsAsync()
    {
        _logger.LogInformation("Seeding Projects...");

        var project1Result = Project.Create(
            ProjectCode.Create("PRJ-2024-001").Value,
            "E-Commerce Platform",
            "Sistema completo de comercio electrónico con carrito de compras, pagos y gestión de inventario",
            DateTime.UtcNow.AddMonths(-6),
            "John Doe"
        );

        var project2Result = Project.Create(
            ProjectCode.Create("PRJ-2024-002").Value,
            "Customer Portal",
            "Portal de autogestión para clientes con seguimiento de pedidos y soporte",
            DateTime.UtcNow.AddMonths(-3),
            "Mary Johnson"
        );

        var project3Result = Project.Create(
            ProjectCode.Create("PRJ-2024-003").Value,
            "Analytics Dashboard",
            "Dashboard de análisis y reporting en tiempo real para métricas de negocio",
            DateTime.UtcNow.AddMonths(-2),
            "Robert Smith"
        );

        var projects = new List<Project>
        {
            project1Result.Value,
            project2Result.Value,
            project3Result.Value
        };

        await _context.Projects.AddRangeAsync(projects);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {projects.Count} projects");
        return projects;
    }

    private async Task<List<Domain.Entities.Application>> SeedApplicationsAsync(List<Project> projects)
    {
        _logger.LogInformation("Seeding Applications...");

        var app1Result = Domain.Entities.Application.Create(
            projects[0].Id,
            "Web Application",
            "Frontend web con React y TypeScript",
            ApplicationType.New,
            ApplicationVersion.Create("1.2.0").Value
        );

        var app2Result = Domain.Entities.Application.Create(
            projects[0].Id,
            "API Backend",
            "API REST con .NET 9 y Clean Architecture",
            ApplicationType.New,
            ApplicationVersion.Create("2.1.3").Value
        );

        var app3Result = Domain.Entities.Application.Create(
            projects[0].Id,
            "Mobile App",
            "Aplicación móvil con React Native",
            ApplicationType.New,
            ApplicationVersion.Create("1.0.5").Value
        );

        var app4Result = Domain.Entities.Application.Create(
            projects[1].Id,
            "Portal Web",
            "Portal de clientes con Angular",
            ApplicationType.Modified,
            ApplicationVersion.Create("1.5.0").Value
        );

        var app5Result = Domain.Entities.Application.Create(
            projects[1].Id,
            "Portal API",
            "API para portal de clientes",
            ApplicationType.Modified,
            ApplicationVersion.Create("1.5.2").Value
        );

        var app6Result = Domain.Entities.Application.Create(
            projects[2].Id,
            "Analytics UI",
            "Dashboard con Vue.js y Chart.js",
            ApplicationType.New,
            ApplicationVersion.Create("0.9.0").Value
        );

        var applications = new List<Domain.Entities.Application>
        {
            app1Result.Value,
            app2Result.Value,
            app3Result.Value,
            app4Result.Value,
            app5Result.Value,
            app6Result.Value
        };

        await _context.Applications.AddRangeAsync(applications);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {applications.Count} applications");
        return applications;
    }

    private async Task<List<Capability>> SeedCapabilitiesAsync(List<Domain.Entities.Application> applications)
    {
        _logger.LogInformation("Seeding Capabilities...");

        var cap1Result = Capability.Create(
            applications[0].Id,
            "User Authentication",
            "Login, registro y recuperación de contraseña",
            CapabilityCategory.Security,
            Priority.High
        );

        var cap2Result = Capability.Create(
            applications[0].Id,
            "Product Catalog",
            "Búsqueda, filtrado y visualización de productos",
            CapabilityCategory.Feature,
            Priority.High
        );

        var cap3Result = Capability.Create(
            applications[0].Id,
            "Shopping Cart",
            "Agregar, eliminar y modificar productos en el carrito",
            CapabilityCategory.Feature,
            Priority.High
        );

        var cap4Result = Capability.Create(
            applications[1].Id,
            "Payment Processing",
            "Integración con pasarelas de pago (Stripe, PayPal)",
            CapabilityCategory.Integration,
            Priority.Critical
        );

        var cap5Result = Capability.Create(
            applications[1].Id,
            "Order Management",
            "Gestión completa de pedidos y estados",
            CapabilityCategory.Feature,
            Priority.High
        );

        var cap6Result = Capability.Create(
            applications[1].Id,
            "Inventory Control",
            "Control de stock y alertas de inventario",
            CapabilityCategory.Feature,
            Priority.High
        );

        var cap7Result = Capability.Create(
            applications[2].Id,
            "Push Notifications",
            "Notificaciones push para ofertas y actualizaciones",
            CapabilityCategory.Feature,
            Priority.Medium
        );

        var cap8Result = Capability.Create(
            applications[3].Id,
            "Order Tracking",
            "Seguimiento en tiempo real de pedidos",
            CapabilityCategory.Feature,
            Priority.High
        );

        var capabilities = new List<Capability>
        {
            cap1Result.Value,
            cap2Result.Value,
            cap3Result.Value,
            cap4Result.Value,
            cap5Result.Value,
            cap6Result.Value,
            cap7Result.Value,
            cap8Result.Value
        };

        await _context.Capabilities.AddRangeAsync(capabilities);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {capabilities.Count} capabilities");
        return capabilities;
    }

    private async Task<List<BusinessRule>> SeedBusinessRulesAsync(List<Capability> capabilities)
    {
        _logger.LogInformation("Seeding Business Rules...");

        var rule1Result = BusinessRule.Create(
            capabilities[0].Id,
            RuleCode.Create("BR-AUTH-001").Value,
            "Password Strength",
            "Las contraseñas deben tener mínimo 8 caracteres, una mayúscula, una minúscula y un número",
            BusinessRuleType.Validation,
            Priority.High
        );

        var rule2Result = BusinessRule.Create(
            capabilities[0].Id,
            RuleCode.Create("BR-AUTH-002").Value,
            "Session Timeout",
            "Las sesiones inactivas deben expirar después de 30 minutos",
            BusinessRuleType.Authorization,
            Priority.Medium
        );

        var rule3Result = BusinessRule.Create(
            capabilities[2].Id,
            RuleCode.Create("BR-CART-001").Value,
            "Maximum Cart Items",
            "El carrito no puede contener más de 50 productos diferentes",
            BusinessRuleType.Validation,
            Priority.Low
        );

        var rule4Result = BusinessRule.Create(
            capabilities[3].Id,
            RuleCode.Create("BR-PAY-001").Value,
            "Minimum Order Amount",
            "El monto mínimo de pedido es $10 USD",
            BusinessRuleType.Calculation,
            Priority.High
        );

        var rule5Result = BusinessRule.Create(
            capabilities[4].Id,
            RuleCode.Create("BR-ORD-001").Value,
            "Order Cancellation Window",
            "Los pedidos pueden cancelarse dentro de las primeras 2 horas",
            BusinessRuleType.Workflow,
            Priority.Medium
        );

        var rule6Result = BusinessRule.Create(
            capabilities[5].Id,
            RuleCode.Create("BR-INV-001").Value,
            "Low Stock Alert",
            "Enviar alerta cuando el stock sea menor al 10% del stock mínimo",
            BusinessRuleType.Calculation,
            Priority.Medium
        );

        var businessRules = new List<BusinessRule>
        {
            rule1Result.Value,
            rule2Result.Value,
            rule3Result.Value,
            rule4Result.Value,
            rule5Result.Value,
            rule6Result.Value
        };

        // Activar algunas reglas
        businessRules[0].Activate();
        businessRules[1].Activate();
        businessRules[3].Activate();
        businessRules[5].Activate();

        await _context.BusinessRules.AddRangeAsync(businessRules);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {businessRules.Count} business rules");
        return businessRules;
    }

    private async Task<List<WikiPage>> SeedWikiPagesAsync(List<User> users)
    {
        _logger.LogInformation("Seeding Wiki Pages...");

        var wiki1Result = WikiPage.Create(
            "Getting Started",
            "# Getting Started\n\n## Introducción\nBienvenido a la documentación del proyecto E-Commerce Platform.\n\n## Requisitos\n- Node.js 18+\n- .NET 9\n- SQL Server 2022\n\n## Instalación\n```bash\nnpm install\ndotnet restore\n```",
            "Documentation",
            users[1].Id.ToString()
        );

        var wiki2Result = WikiPage.Create(
            "Architecture Overview",
            "# Arquitectura del Sistema\n\n## Clean Architecture\nEl proyecto sigue los principios de Clean Architecture con 4 capas:\n\n1. **Domain**: Entidades y lógica de negocio\n2. **Application**: Casos de uso (CQRS)\n3. **Infrastructure**: Implementaciones de persistencia\n4. **API**: Controllers y endpoints REST",
            "Architecture",
            users[1].Id.ToString()
        );

        var wiki3Result = WikiPage.Create(
            "API Documentation",
            "# API del Portal de Clientes\n\n## Endpoints Principales\n\n### Autenticación\n- POST /api/auth/login\n- POST /api/auth/register\n\n### Pedidos\n- GET /api/orders\n- GET /api/orders/{id}",
            "API",
            users[2].Id.ToString()
        );

        var wiki4Result = WikiPage.Create(
            "Troubleshooting Guide",
            "# Resolución de Problemas Comunes\n\n## Error de Conexión a BD\n**Síntoma**: No se puede conectar a SQL Server\n\n**Solución**:\n1. Verificar que Docker esté corriendo\n2. Verificar connection string\n3. Ejecutar migraciones",
            "Support",
            users[3].Id.ToString()
        );

        var wikiPages = new List<WikiPage>
        {
            wiki1Result.Value,
            wiki2Result.Value,
            wiki3Result.Value,
            wiki4Result.Value
        };

        // Publicar algunas páginas
        wikiPages[0].Publish();
        wikiPages[1].Publish();
        wikiPages[2].Publish();

        // Agregar tags
        wikiPages[0].AddTag("getting-started");
        wikiPages[0].AddTag("documentation");
        wikiPages[1].AddTag("architecture");
        wikiPages[1].AddTag("clean-architecture");
        wikiPages[2].AddTag("api");
        wikiPages[2].AddTag("documentation");

        await _context.WikiPages.AddRangeAsync(wikiPages);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {wikiPages.Count} wiki pages");
        return wikiPages;
    }

    private async Task SeedNotificationsAsync(List<User> users)
    {
        _logger.LogInformation("Seeding Notifications...");

        var notifications = new List<Notification>
        {
            Notification.Create(
                "Bienvenido al Sistema",
                "Tu cuenta ha sido creada exitosamente. ¡Bienvenido a Clean Architecture API!",
                NotificationType.Success,
                users[1].Id.ToString()
            ),
            Notification.Create(
                "Nuevo Proyecto Creado",
                "El proyecto 'E-Commerce Platform' ha sido creado y asignado a tu equipo.",
                NotificationType.ProjectCreated,
                users[1].Id.ToString(),
                "Project",
                Guid.NewGuid()
            ),
            Notification.Create(
                "Tarea Asignada",
                "Te han asignado la tarea 'Implementar Payment Integration'.",
                NotificationType.Info,
                users[1].Id.ToString()
            ),
            Notification.Create(
                "Sistema Actualizado",
                "El sistema se actualizará mañana a las 2 AM. Duración estimada: 30 minutos.",
                NotificationType.Warning,
                null // A todos los usuarios
            ),
            Notification.Create(
                "Nueva Funcionalidad",
                "Ya está disponible el módulo de exportación a Excel. ¡Pruébalo!",
                NotificationType.Success,
                null
            )
        };

        // Marcar primera notificación como leída
        notifications[0].MarkAsRead();

        await _context.Notifications.AddRangeAsync(notifications);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Seeded {notifications.Count} notifications");
    }
}

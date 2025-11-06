using CleanArch.Domain.Common;
using CleanArch.Domain.Enums;
using CleanArch.Domain.ValueObjects;

namespace CleanArch.Domain.Entities;

/// <summary>
/// Agregado raíz que representa una aplicación dentro de un proyecto
/// </summary>
public class Application : BaseAuditableEntity
{
    private readonly List<Capability> _capabilities = new();

    private Application() { } // EF Core

    private Application(
        Guid projectId,
        string name,
        string description,
        ApplicationType type,
        ApplicationVersion version)
    {
        ProjectId = projectId;
        Name = name;
        Description = description;
        Type = type;
        Version = version;
        Status = ApplicationStatus.Planning;
    }

    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public ApplicationType Type { get; private set; }
    public ApplicationVersion Version { get; private set; } = null!;
    public ApplicationStatus Status { get; private set; }
    public string? TechnologyStack { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public IReadOnlyCollection<Capability> Capabilities => _capabilities.AsReadOnly();

    public static Result<Application> Create(
        Guid projectId,
        string name,
        string description,
        ApplicationType type,
        ApplicationVersion version)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Application>.Failure("Application name cannot be empty");

        if (name.Length > 200)
            return Result<Application>.Failure("Application name cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(description))
            return Result<Application>.Failure("Application description cannot be empty");

        if (projectId == Guid.Empty)
            return Result<Application>.Failure("Project ID cannot be empty");

        var application = new Application(projectId, name.Trim(), description.Trim(), type, version);

        return Result<Application>.Success(application);
    }

    public Result AddCapability(Capability capability)
    {
        if (capability == null)
            return Result.Failure("Capability cannot be null");

        if (_capabilities.Any(c => c.Id == capability.Id))
            return Result.Failure("Capability already exists in this application");

        _capabilities.Add(capability);
        return Result.Success();
    }
}

using CleanArch.Domain.Common;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Events;
using CleanArch.Domain.ValueObjects;

namespace CleanArch.Domain.Entities;

/// <summary>
/// Agregado raíz que representa un proyecto de software
/// </summary>
public class Project : BaseAuditableEntity
{
    private readonly List<Application> _applications = new();

    private Project() { } // EF Core

    private Project(
        ProjectCode code,
        string name,
        string description,
        DateTime startDate,
        string projectManager)
    {
        Code = code;
        Name = name;
        Description = description;
        StartDate = startDate;
        ProjectManager = projectManager;
        Status = ProjectStatus.Planning;
    }

    public ProjectCode Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public ProjectStatus Status { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime? PlannedEndDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public string ProjectManager { get; private set; } = null!;

    public IReadOnlyCollection<Application> Applications => _applications.AsReadOnly();

    public static Result<Project> Create(
        ProjectCode code,
        string name,
        string description,
        DateTime startDate,
        string projectManager)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Project>.Failure("Project name cannot be empty");

        if (name.Length > 200)
            return Result<Project>.Failure("Project name cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(description))
            return Result<Project>.Failure("Project description cannot be empty");

        if (string.IsNullOrWhiteSpace(projectManager))
            return Result<Project>.Failure("Project manager cannot be empty");

        var project = new Project(code, name.Trim(), description.Trim(), startDate, projectManager.Trim());

        project.AddDomainEvent(new ProjectCreatedEvent(
            project.Id,
            project.Code.Value,
            project.Name));

        return Result<Project>.Success(project);
    }

    public Result ChangeStatus(ProjectStatus newStatus)
    {
        if (Status == newStatus)
            return Result.Failure($"Project is already in {newStatus} status");

        var oldStatus = Status;
        Status = newStatus;

        AddDomainEvent(new ProjectStatusChangedEvent(
            Id,
            oldStatus,
            newStatus));

        return Result.Success();
    }

    public Result SetPlannedEndDate(DateTime plannedEndDate)
    {
        if (plannedEndDate < StartDate)
            return Result.Failure("Planned end date cannot be before start date");

        PlannedEndDate = plannedEndDate;
        return Result.Success();
    }

    public Result Complete(DateTime actualEndDate)
    {
        if (actualEndDate < StartDate)
            return Result.Failure("Actual end date cannot be before start date");

        ActualEndDate = actualEndDate;
        Status = ProjectStatus.Completed;

        AddDomainEvent(new ProjectStatusChangedEvent(
            Id,
            Status,
            ProjectStatus.Completed));

        return Result.Success();
    }

    public void Cancel()
    {
        var oldStatus = Status;
        Status = ProjectStatus.Cancelled;

        AddDomainEvent(new ProjectStatusChangedEvent(
            Id,
            oldStatus,
            ProjectStatus.Cancelled));
    }

    public Result UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Project name cannot be empty");

        if (name.Length > 200)
            return Result.Failure("Project name cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure("Project description cannot be empty");

        Name = name.Trim();
        Description = description.Trim();

        return Result.Success();
    }

    public Result AddApplication(Application application)
    {
        if (application == null)
            return Result.Failure("Application cannot be null");

        if (_applications.Any(a => a.Id == application.Id))
            return Result.Failure("Application already exists in this project");

        _applications.Add(application);

        AddDomainEvent(new ApplicationAddedToProjectEvent(
            Id,
            application.Id,
            application.Name));

        return Result.Success();
    }

    public Result RemoveApplication(Guid applicationId)
    {
        var application = _applications.FirstOrDefault(a => a.Id == applicationId);
        if (application == null)
            return Result.Failure("Application not found in this project");

        _applications.Remove(application);
        return Result.Success();
    }
}

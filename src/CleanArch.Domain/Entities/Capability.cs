using CleanArch.Domain.Common;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Events;

namespace CleanArch.Domain.Entities;

/// <summary>
/// Entidad que representa una capacidad funcional de una aplicación
/// </summary>
public class Capability : BaseAuditableEntity
{
    private readonly List<BusinessRule> _businessRules = new();

    private Capability() { } // EF Core

    private Capability(
        Guid applicationId,
        string name,
        string description,
        CapabilityCategory category,
        Priority priority)
    {
        ApplicationId = applicationId;
        Name = name;
        Description = description;
        Category = category;
        Priority = priority;
        Status = CapabilityStatus.Planned;
    }

    public Guid ApplicationId { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public CapabilityCategory Category { get; private set; }
    public Priority Priority { get; private set; }
    public CapabilityStatus Status { get; private set; }
    public DateTime? StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }

    public IReadOnlyCollection<BusinessRule> BusinessRules => _businessRules.AsReadOnly();

    public static Result<Capability> Create(
        Guid applicationId,
        string name,
        string description,
        CapabilityCategory category,
        Priority priority)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Capability>.Failure("Capability name cannot be empty");

        if (name.Length > 200)
            return Result<Capability>.Failure("Capability name cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(description))
            return Result<Capability>.Failure("Capability description cannot be empty");

        if (applicationId == Guid.Empty)
            return Result<Capability>.Failure("Application ID cannot be empty");

        var capability = new Capability(applicationId, name.Trim(), description.Trim(), category, priority);

        capability.AddDomainEvent(new CapabilityCreatedEvent(
            capability.Id,
            capability.ApplicationId,
            capability.Name));

        return Result<Capability>.Success(capability);
    }

    public Result ChangeStatus(CapabilityStatus newStatus)
    {
        if (Status == newStatus)
            return Result.Failure($"Capability is already in {newStatus} status");

        Status = newStatus;
        return Result.Success();
    }

    public void Complete()
    {
        Status = CapabilityStatus.Completed;
        EndDate = DateTime.UtcNow;
    }

    public Result SetDates(DateTime startDate, DateTime? endDate)
    {
        if (endDate.HasValue && endDate.Value < startDate)
            return Result.Failure("End date cannot be before start date");

        StartDate = startDate;
        EndDate = endDate;

        return Result.Success();
    }

    public Result AddBusinessRule(BusinessRule businessRule)
    {
        if (businessRule == null)
            return Result.Failure("Business rule cannot be null");

        if (_businessRules.Any(r => r.Id == businessRule.Id))
            return Result.Failure("Business rule already exists in this capability");

        _businessRules.Add(businessRule);
        return Result.Success();
    }

    public Result RemoveBusinessRule(Guid businessRuleId)
    {
        var rule = _businessRules.FirstOrDefault(r => r.Id == businessRuleId);
        if (rule == null)
            return Result.Failure("Business rule not found in this capability");

        _businessRules.Remove(rule);
        return Result.Success();
    }

    public Result UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Capability name cannot be empty");

        if (name.Length > 200)
            return Result.Failure("Capability name cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure("Capability description cannot be empty");

        Name = name.Trim();
        Description = description.Trim();

        return Result.Success();
    }
}

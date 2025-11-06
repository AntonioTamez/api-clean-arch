using CleanArch.Domain.Common;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Events;
using CleanArch.Domain.ValueObjects;

namespace CleanArch.Domain.Entities;

/// <summary>
/// Entidad que representa una regla de negocio asociada a una capacidad
/// </summary>
public class BusinessRule : BaseAuditableEntity
{
    private readonly List<string> _examples = new();

    private BusinessRule() { } // EF Core

    private BusinessRule(
        Guid capabilityId,
        RuleCode code,
        string name,
        string description,
        BusinessRuleType ruleType,
        Priority priority)
    {
        CapabilityId = capabilityId;
        Code = code;
        Name = name;
        Description = description;
        RuleType = ruleType;
        Priority = priority;
        Status = BusinessRuleStatus.Active;
    }

    public Guid CapabilityId { get; private set; }
    public RuleCode Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public BusinessRuleType RuleType { get; private set; }
    public string? Implementation { get; private set; }
    public Priority Priority { get; private set; }
    public BusinessRuleStatus Status { get; private set; }

    public IReadOnlyCollection<string> Examples => _examples.AsReadOnly();

    public static Result<BusinessRule> Create(
        Guid capabilityId,
        RuleCode code,
        string name,
        string description,
        BusinessRuleType ruleType,
        Priority priority)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<BusinessRule>.Failure("Business rule name cannot be empty");

        if (name.Length > 200)
            return Result<BusinessRule>.Failure("Business rule name cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(description))
            return Result<BusinessRule>.Failure("Business rule description cannot be empty");

        if (capabilityId == Guid.Empty)
            return Result<BusinessRule>.Failure("Capability ID cannot be empty");

        var businessRule = new BusinessRule(capabilityId, code, name.Trim(), description.Trim(), ruleType, priority);

        businessRule.AddDomainEvent(new BusinessRuleCreatedEvent(
            businessRule.Id,
            businessRule.CapabilityId,
            businessRule.Code.Value,
            businessRule.Name));

        return Result<BusinessRule>.Success(businessRule);
    }

    public void Activate()
    {
        if (Status == BusinessRuleStatus.Active)
            return;

        var oldStatus = Status;
        Status = BusinessRuleStatus.Active;

        AddDomainEvent(new BusinessRuleStatusChangedEvent(
            Id,
            oldStatus,
            Status));
    }

    public void Deactivate()
    {
        if (Status == BusinessRuleStatus.Inactive)
            return;

        var oldStatus = Status;
        Status = BusinessRuleStatus.Inactive;

        AddDomainEvent(new BusinessRuleStatusChangedEvent(
            Id,
            oldStatus,
            Status));
    }

    public void Deprecate()
    {
        if (Status == BusinessRuleStatus.Deprecated)
            return;

        var oldStatus = Status;
        Status = BusinessRuleStatus.Deprecated;

        AddDomainEvent(new BusinessRuleStatusChangedEvent(
            Id,
            oldStatus,
            Status));
    }

    public Result SetImplementation(string implementation)
    {
        if (string.IsNullOrWhiteSpace(implementation))
            return Result.Failure("Implementation cannot be empty");

        if (implementation.Length > 4000)
            return Result.Failure("Implementation cannot exceed 4000 characters");

        Implementation = implementation.Trim();
        return Result.Success();
    }

    public Result AddExample(string example)
    {
        if (string.IsNullOrWhiteSpace(example))
            return Result.Failure("Example cannot be empty");

        if (example.Length > 1000)
            return Result.Failure("Example cannot exceed 1000 characters");

        _examples.Add(example.Trim());
        return Result.Success();
    }

    public Result UpdateDetails(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Business rule name cannot be empty");

        if (name.Length > 200)
            return Result.Failure("Business rule name cannot exceed 200 characters");

        if (string.IsNullOrWhiteSpace(description))
            return Result.Failure("Business rule description cannot be empty");

        Name = name.Trim();
        Description = description.Trim();

        return Result.Success();
    }
}

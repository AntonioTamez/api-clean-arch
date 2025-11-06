using CleanArch.Domain.Entities;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Events;
using CleanArch.Domain.ValueObjects;
using FluentAssertions;

namespace CleanArch.Domain.Tests.Entities;

public class BusinessRuleTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var capabilityId = Guid.NewGuid();
        var code = RuleCode.Create("BR-VAL-001").Value;
        var name = "Email Validation Rule";
        var description = "Validates email format";
        var type = BusinessRuleType.Validation;
        var priority = Priority.High;

        // Act
        var result = BusinessRule.Create(capabilityId, code, name, description, type, priority);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.CapabilityId.Should().Be(capabilityId);
        result.Value.Code.Should().Be(code);
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(description);
        result.Value.RuleType.Should().Be(type);
        result.Value.Priority.Should().Be(priority);
        result.Value.Status.Should().Be(BusinessRuleStatus.Active);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldFail(string invalidName)
    {
        // Arrange
        var capabilityId = Guid.NewGuid();
        var code = RuleCode.Create("BR-VAL-001").Value;

        // Act
        var result = BusinessRule.Create(capabilityId, code, invalidName, "Description", BusinessRuleType.Validation, Priority.Medium);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("name");
    }

    [Fact]
    public void Create_ShouldRaiseBusinessRuleCreatedEvent()
    {
        // Arrange
        var capabilityId = Guid.NewGuid();
        var code = RuleCode.Create("BR-VAL-001").Value;

        // Act
        var result = BusinessRule.Create(capabilityId, code, "Test Rule", "Description", BusinessRuleType.Validation, Priority.Medium);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle();
        result.Value.DomainEvents.First().Should().BeOfType<BusinessRuleCreatedEvent>();
    }

    [Fact]
    public void Activate_WhenInactive_ShouldChangeStatus()
    {
        // Arrange
        var rule = CreateValidBusinessRule();
        rule.Deactivate();

        // Act
        rule.Activate();

        // Assert
        rule.Status.Should().Be(BusinessRuleStatus.Active);
    }

    [Fact]
    public void Activate_ShouldRaiseStatusChangedEvent()
    {
        // Arrange
        var rule = CreateValidBusinessRule();
        rule.Deactivate();
        rule.ClearDomainEvents(); // Limpiar eventos anteriores

        // Act
        rule.Activate();

        // Assert
        var statusEvent = rule.DomainEvents.OfType<BusinessRuleStatusChangedEvent>().FirstOrDefault();
        statusEvent.Should().NotBeNull();
        statusEvent!.NewStatus.Should().Be(BusinessRuleStatus.Active);
    }

    [Fact]
    public void Deactivate_WhenActive_ShouldChangeStatus()
    {
        // Arrange
        var rule = CreateValidBusinessRule();

        // Act
        rule.Deactivate();

        // Assert
        rule.Status.Should().Be(BusinessRuleStatus.Inactive);
    }

    [Fact]
    public void Deprecate_ShouldChangeStatusToDeprecated()
    {
        // Arrange
        var rule = CreateValidBusinessRule();

        // Act
        rule.Deprecate();

        // Assert
        rule.Status.Should().Be(BusinessRuleStatus.Deprecated);
    }

    [Fact]
    public void SetImplementation_WithValidText_ShouldSucceed()
    {
        // Arrange
        var rule = CreateValidBusinessRule();
        var implementation = "if (email.Contains('@')) return true;";

        // Act
        var result = rule.SetImplementation(implementation);

        // Assert
        result.IsSuccess.Should().BeTrue();
        rule.Implementation.Should().Be(implementation);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetImplementation_WithInvalidText_ShouldFail(string invalidImpl)
    {
        // Arrange
        var rule = CreateValidBusinessRule();

        // Act
        var result = rule.SetImplementation(invalidImpl);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void AddExample_WithValidExample_ShouldSucceed()
    {
        // Arrange
        var rule = CreateValidBusinessRule();
        var example = "Valid: user@example.com, Invalid: userexample.com";

        // Act
        var result = rule.AddExample(example);

        // Assert
        result.IsSuccess.Should().BeTrue();
        rule.Examples.Should().Contain(example);
    }

    private BusinessRule CreateValidBusinessRule()
    {
        var capabilityId = Guid.NewGuid();
        var code = RuleCode.Create("BR-VAL-001").Value;
        return BusinessRule.Create(
            capabilityId,
            code,
            "Test Rule",
            "Test Description",
            BusinessRuleType.Validation,
            Priority.Medium).Value;
    }
}

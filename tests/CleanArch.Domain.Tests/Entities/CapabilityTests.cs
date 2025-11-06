using CleanArch.Domain.Entities;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Events;
using FluentAssertions;

namespace CleanArch.Domain.Tests.Entities;

public class CapabilityTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var applicationId = Guid.NewGuid();
        var name = "User Authentication";
        var description = "Capability to authenticate users";
        var category = CapabilityCategory.Security;
        var priority = Priority.Critical;

        // Act
        var result = Capability.Create(applicationId, name, description, category, priority);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.ApplicationId.Should().Be(applicationId);
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(description);
        result.Value.Category.Should().Be(category);
        result.Value.Priority.Should().Be(priority);
        result.Value.Status.Should().Be(CapabilityStatus.Planned);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldFail(string invalidName)
    {
        // Arrange
        var applicationId = Guid.NewGuid();

        // Act
        var result = Capability.Create(applicationId, invalidName, "Description", CapabilityCategory.Feature, Priority.Medium);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("name");
    }

    [Fact]
    public void Create_ShouldRaiseCapabilityCreatedEvent()
    {
        // Arrange
        var applicationId = Guid.NewGuid();

        // Act
        var result = Capability.Create(applicationId, "Test Capability", "Description", CapabilityCategory.Feature, Priority.Medium);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle();
        result.Value.DomainEvents.First().Should().BeOfType<CapabilityCreatedEvent>();
    }

    [Fact]
    public void ChangeStatus_WithValidStatus_ShouldSucceed()
    {
        // Arrange
        var capability = CreateValidCapability();

        // Act
        var result = capability.ChangeStatus(CapabilityStatus.InProgress);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capability.Status.Should().Be(CapabilityStatus.InProgress);
    }

    [Fact]
    public void ChangeStatus_ToSameStatus_ShouldFail()
    {
        // Arrange
        var capability = CreateValidCapability();
        var currentStatus = capability.Status;

        // Act
        var result = capability.ChangeStatus(currentStatus);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Complete_ShouldChangeStatusToCompleted()
    {
        // Arrange
        var capability = CreateValidCapability();
        capability.ChangeStatus(CapabilityStatus.InProgress);

        // Act
        capability.Complete();

        // Assert
        capability.Status.Should().Be(CapabilityStatus.Completed);
    }

    [Fact]
    public void SetDates_WithValidDates_ShouldSucceed()
    {
        // Arrange
        var capability = CreateValidCapability();
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddMonths(2);

        // Act
        var result = capability.SetDates(startDate, endDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capability.StartDate.Should().Be(startDate);
        capability.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void SetDates_WithEndDateBeforeStartDate_ShouldFail()
    {
        // Arrange
        var capability = CreateValidCapability();
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(-1);

        // Act
        var result = capability.SetDates(startDate, endDate);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void AddBusinessRule_WithValidRule_ShouldSucceed()
    {
        // Arrange
        var capability = CreateValidCapability();
        var businessRule = CreateValidBusinessRule(capability.Id);

        // Act
        var result = capability.AddBusinessRule(businessRule);

        // Assert
        result.IsSuccess.Should().BeTrue();
        capability.BusinessRules.Should().Contain(businessRule);
    }

    [Fact]
    public void AddBusinessRule_WithNull_ShouldFail()
    {
        // Arrange
        var capability = CreateValidCapability();

        // Act
        var result = capability.AddBusinessRule(null!);

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    private Capability CreateValidCapability()
    {
        return Capability.Create(
            Guid.NewGuid(),
            "Test Capability",
            "Test Description",
            CapabilityCategory.Feature,
            Priority.Medium).Value;
    }

    private BusinessRule CreateValidBusinessRule(Guid capabilityId)
    {
        var code = Domain.ValueObjects.RuleCode.Create("BR-TEST-001").Value;
        return BusinessRule.Create(
            capabilityId,
            code,
            "Test Rule",
            "Test Description",
            BusinessRuleType.Validation,
            Priority.Medium).Value;
    }
}

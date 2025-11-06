using CleanArch.Domain.Entities;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Events;
using CleanArch.Domain.ValueObjects;
using FluentAssertions;

namespace CleanArch.Domain.Tests.Entities;

public class ProjectTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var code = ProjectCode.Create("PRJ-2024-001").Value;
        var name = "Clean Architecture API";
        var description = "API implementation using Clean Architecture";
        var startDate = DateTime.UtcNow;
        var manager = "John Doe";

        // Act
        var result = Project.Create(code, name, description, startDate, manager);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Code.Should().Be(code);
        result.Value.Name.Should().Be(name);
        result.Value.Description.Should().Be(description);
        result.Value.StartDate.Should().Be(startDate);
        result.Value.ProjectManager.Should().Be(manager);
        result.Value.Status.Should().Be(ProjectStatus.Planning);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldFail(string invalidName)
    {
        // Arrange
        var code = ProjectCode.Create("PRJ-2024-001").Value;
        var startDate = DateTime.UtcNow;

        // Act
        var result = Project.Create(code, invalidName, "Description", startDate, "Manager");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("name");
    }

    [Fact]
    public void Create_ShouldRaiseProjectCreatedEvent()
    {
        // Arrange
        var code = ProjectCode.Create("PRJ-2024-001").Value;

        // Act
        var result = Project.Create(code, "Test Project", "Description", DateTime.UtcNow, "Manager");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.DomainEvents.Should().ContainSingle();
        result.Value.DomainEvents.First().Should().BeOfType<ProjectCreatedEvent>();
    }

    [Fact]
    public void ChangeStatus_WithValidTransition_ShouldSucceed()
    {
        // Arrange
        var project = CreateValidProject();

        // Act
        var result = project.ChangeStatus(ProjectStatus.InProgress);

        // Assert
        result.IsSuccess.Should().BeTrue();
        project.Status.Should().Be(ProjectStatus.InProgress);
    }

    [Fact]
    public void ChangeStatus_ShouldRaiseStatusChangedEvent()
    {
        // Arrange
        var project = CreateValidProject();
        var oldStatus = project.Status;

        // Act
        project.ChangeStatus(ProjectStatus.InProgress);

        // Assert
        var statusEvent = project.DomainEvents.OfType<ProjectStatusChangedEvent>().FirstOrDefault();
        statusEvent.Should().NotBeNull();
        statusEvent!.OldStatus.Should().Be(oldStatus);
        statusEvent.NewStatus.Should().Be(ProjectStatus.InProgress);
    }

    [Fact]
    public void ChangeStatus_ToSameStatus_ShouldFail()
    {
        // Arrange
        var project = CreateValidProject();
        var currentStatus = project.Status;

        // Act
        var result = project.ChangeStatus(currentStatus);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("already");
    }

    [Fact]
    public void SetPlannedEndDate_WithValidDate_ShouldSucceed()
    {
        // Arrange
        var project = CreateValidProject();
        var plannedEndDate = project.StartDate.AddMonths(6);

        // Act
        var result = project.SetPlannedEndDate(plannedEndDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        project.PlannedEndDate.Should().Be(plannedEndDate);
    }

    [Fact]
    public void SetPlannedEndDate_BeforeStartDate_ShouldFail()
    {
        // Arrange
        var project = CreateValidProject();
        var invalidDate = project.StartDate.AddDays(-1);

        // Act
        var result = project.SetPlannedEndDate(invalidDate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("before");
    }

    [Fact]
    public void Complete_WithValidActualEndDate_ShouldSucceed()
    {
        // Arrange
        var project = CreateValidProject();
        project.ChangeStatus(ProjectStatus.InProgress);
        var actualEndDate = DateTime.UtcNow;

        // Act
        var result = project.Complete(actualEndDate);

        // Assert
        result.IsSuccess.Should().BeTrue();
        project.Status.Should().Be(ProjectStatus.Completed);
        project.ActualEndDate.Should().Be(actualEndDate);
    }

    [Fact]
    public void Complete_BeforeStartDate_ShouldFail()
    {
        // Arrange
        var project = CreateValidProject();
        var invalidDate = project.StartDate.AddDays(-1);

        // Act
        var result = project.Complete(invalidDate);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("before");
    }

    [Fact]
    public void Cancel_ShouldChangeStatusToCancelled()
    {
        // Arrange
        var project = CreateValidProject();

        // Act
        project.Cancel();

        // Assert
        project.Status.Should().Be(ProjectStatus.Cancelled);
    }

    [Fact]
    public void UpdateDetails_WithValidData_ShouldSucceed()
    {
        // Arrange
        var project = CreateValidProject();
        var newName = "Updated Project Name";
        var newDescription = "Updated Description";

        // Act
        var result = project.UpdateDetails(newName, newDescription);

        // Assert
        result.IsSuccess.Should().BeTrue();
        project.Name.Should().Be(newName);
        project.Description.Should().Be(newDescription);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_WithInvalidName_ShouldFail(string invalidName)
    {
        // Arrange
        var project = CreateValidProject();

        // Act
        var result = project.UpdateDetails(invalidName, "Description");

        // Assert
        result.IsFailure.Should().BeTrue();
    }

    // Helper method
    private Project CreateValidProject()
    {
        var code = ProjectCode.Create("PRJ-2024-001").Value;
        return Project.Create(
            code,
            "Test Project",
            "Test Description",
            DateTime.UtcNow,
            "Test Manager").Value;
    }
}

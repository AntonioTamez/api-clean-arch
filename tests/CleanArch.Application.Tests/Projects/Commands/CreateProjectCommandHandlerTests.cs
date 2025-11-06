using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Projects.Commands.CreateProject;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CleanArch.Application.Tests.Projects.Commands;

public class CreateProjectCommandHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateProjectCommandHandler _handler;

    public CreateProjectCommandHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateProjectCommandHandler(
            _projectRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesProject()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Code = "PRJ-2024-001",
            Name = "Clean Architecture API",
            Description = "API implementation using Clean Architecture",
            StartDate = DateTime.UtcNow,
            ProjectManager = "John Doe"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        
        _projectRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()),
            Times.Once);
        
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateCode_ReturnsFailure()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Code = "PRJ-2024-001",
            Name = "Test Project",
            Description = "Description",
            StartDate = DateTime.UtcNow,
            ProjectManager = "Manager"
        };

        var existingProject = Project.Create(
            Domain.ValueObjects.ProjectCode.Create("PRJ-2024-001").Value,
            "Existing Project",
            "Description",
            DateTime.UtcNow,
            "Manager").Value;

        _projectRepositoryMock
            .Setup(x => x.GetByCodeAsync(command.Code, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProject);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("already exists");
        
        _projectRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidProjectCode_ReturnsFailure()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Code = "AB", // Muy corto
            Name = "Test Project",
            Description = "Description",
            StartDate = DateTime.UtcNow,
            ProjectManager = "Manager"
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("code");
    }

    [Fact]
    public async Task Handle_PlannedEndDateBeforeStartDate_ReturnsFailure()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var command = new CreateProjectCommand
        {
            Code = "PRJ-2024-001",
            Name = "Test Project",
            Description = "Description",
            StartDate = startDate,
            PlannedEndDate = startDate.AddDays(-1), // Antes de StartDate
            ProjectManager = "Manager"
        };

        _projectRepositoryMock
            .Setup(x => x.GetByCodeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("before");
    }
}

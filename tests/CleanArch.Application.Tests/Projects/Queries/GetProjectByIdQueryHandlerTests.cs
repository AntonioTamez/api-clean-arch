using AutoMapper;
using CleanArch.Application.Projects.DTOs;
using CleanArch.Application.Projects.Queries.GetProjectById;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;
using CleanArch.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CleanArch.Application.Tests.Projects.Queries;

public class GetProjectByIdQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetProjectByIdQueryHandler _handler;

    public GetProjectByIdQueryHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetProjectByIdQueryHandler(
            _projectRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingProject_ReturnsProjectDto()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = CreateValidProject();
        var projectDto = new ProjectDto
        {
            Id = project.Id,
            Code = project.Code.Value,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status.ToString(),
            StartDate = project.StartDate,
            ProjectManager = project.ProjectManager,
            ApplicationCount = 0
        };

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _mapperMock
            .Setup(x => x.Map<ProjectDto>(project))
            .Returns(projectDto);

        var query = new GetProjectByIdQuery(projectId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(project.Id);
        result.Value.Code.Should().Be(project.Code.Value);
    }

    [Fact]
    public async Task Handle_NonExistingProject_ReturnsFailure()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _projectRepositoryMock
            .Setup(x => x.GetByIdAsync(projectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project?)null);

        var query = new GetProjectByIdQuery(projectId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("not found");
    }

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

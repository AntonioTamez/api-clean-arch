using AutoMapper;
using CleanArch.Application.Projects.DTOs;
using CleanArch.Application.Projects.Queries.GetProjects;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Interfaces;
using CleanArch.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace CleanArch.Application.Tests.Projects.Queries;

public class GetProjectsQueryHandlerTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetProjectsQueryHandler _handler;

    public GetProjectsQueryHandlerTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetProjectsQueryHandler(
            _projectRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            CreateValidProject("PRJ-001", "Project 1"),
            CreateValidProject("PRJ-002", "Project 2"),
            CreateValidProject("PRJ-003", "Project 3")
        };

        var projectDtos = projects.Select(p => new ProjectListItemDto
        {
            Id = p.Id,
            Code = p.Code.Value,
            Name = p.Name,
            Status = p.Status.ToString(),
            StartDate = p.StartDate,
            ProjectManager = p.ProjectManager,
            ApplicationCount = 0
        }).ToList();

        _projectRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        _mapperMock
            .Setup(x => x.Map<List<ProjectListItemDto>>(It.IsAny<List<Project>>()))
            .Returns(projectDtos);

        var query = new GetProjectsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ReturnsFilteredProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            CreateValidProject("PRJ-001", "Project 1"),
            CreateValidProject("PRJ-002", "Project 2")
        };

        projects[0].ChangeStatus(ProjectStatus.InProgress);

        var filteredProjects = projects.Where(p => p.Status == ProjectStatus.InProgress).ToList();

        var projectDtos = filteredProjects.Select(p => new ProjectListItemDto
        {
            Id = p.Id,
            Code = p.Code.Value,
            Name = p.Name,
            Status = p.Status.ToString(),
            StartDate = p.StartDate,
            ProjectManager = p.ProjectManager,
            ApplicationCount = 0
        }).ToList();

        _projectRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(projects);

        _mapperMock
            .Setup(x => x.Map<List<ProjectListItemDto>>(It.IsAny<List<Project>>()))
            .Returns(projectDtos);

        var query = new GetProjectsQuery { Status = ProjectStatus.InProgress };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value.First().Status.Should().Be(ProjectStatus.InProgress.ToString());
    }

    [Fact]
    public async Task Handle_EmptyRepository_ReturnsEmptyList()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Project>());

        _mapperMock
            .Setup(x => x.Map<List<ProjectListItemDto>>(It.IsAny<List<Project>>()))
            .Returns(new List<ProjectListItemDto>());

        var query = new GetProjectsQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    private Project CreateValidProject(string code, string name)
    {
        var projectCode = ProjectCode.Create(code).Value;
        return Project.Create(
            projectCode,
            name,
            "Test Description",
            DateTime.UtcNow,
            "Test Manager").Value;
    }
}

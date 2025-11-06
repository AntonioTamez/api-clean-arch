using CleanArch.API.Controllers;
using CleanArch.Application.Common.Models;
using CleanArch.Application.Projects.Commands.CreateProject;
using CleanArch.Application.Projects.DTOs;
using CleanArch.Application.Projects.Queries.GetProjectById;
using CleanArch.Application.Projects.Queries.GetProjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CleanArch.API.Tests.Controllers;

public class ProjectsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ProjectsController _controller;

    public ProjectsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ProjectsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithProjects()
    {
        // Arrange
        var projects = new List<ProjectListItemDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Project 1", Code = "PRJ-001" },
            new() { Id = Guid.NewGuid(), Name = "Project 2", Code = "PRJ-002" }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProjectsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<List<ProjectListItemDto>>.Success(projects));

        // Act
        var result = await _controller.GetAll(null, null, null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProjects = Assert.IsAssignableFrom<List<ProjectListItemDto>>(okResult.Value);
        Assert.Equal(2, returnedProjects.Count);
    }

    [Fact]
    public async Task GetById_ExistingProject_ReturnsOk()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new ProjectDto
        {
            Id = projectId,
            Name = "Test Project",
            Code = "PRJ-001"
        };

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetProjectByIdQuery>(q => q.ProjectId == projectId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto>.Success(project));

        // Act
        var result = await _controller.GetById(projectId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProject = Assert.IsType<ProjectDto>(okResult.Value);
        Assert.Equal(projectId, returnedProject.Id);
    }

    [Fact]
    public async Task GetById_NonExistingProject_ReturnsNotFound()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.Is<GetProjectByIdQuery>(q => q.ProjectId == projectId), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProjectDto>.Failure(new Error("NotFound", "Project not found")));

        // Act
        var result = await _controller.GetById(projectId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_ValidCommand_ReturnsCreated()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var command = new CreateProjectCommand
        {
            Code = "PRJ-001",
            Name = "New Project",
            Description = "Description",
            StartDate = DateTime.UtcNow,
            ProjectManager = "Manager"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Success(projectId));

        // Act
        var result = await _controller.Create(command);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(projectId, createdResult.Value);
        Assert.Equal(nameof(_controller.GetById), createdResult.ActionName);
    }

    [Fact]
    public async Task Create_InvalidCommand_ReturnsBadRequest()
    {
        // Arrange
        var command = new CreateProjectCommand
        {
            Code = "AB",
            Name = "Test",
            Description = "Desc",
            StartDate = DateTime.UtcNow,
            ProjectManager = "Manager"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateProjectCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Guid>.Failure(new Error("Validation", "Invalid code")));

        // Act
        var result = await _controller.Create(command);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}

using CleanArch.Application.Projects.Commands.CreateProject;
using CleanArch.Application.Projects.DTOs;
using CleanArch.Application.Projects.Queries.GetProjectById;
using CleanArch.Application.Projects.Queries.GetProjects;
using CleanArch.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

/// <summary>
/// Controller para gestión de proyectos
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProjectsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todos los proyectos con filtros opcionales
    /// </summary>
    /// <param name="status">Filtro por estado</param>
    /// <param name="searchTerm">Término de búsqueda</param>
    /// <param name="startDateFrom">Fecha inicio desde</param>
    /// <param name="startDateTo">Fecha inicio hasta</param>
    /// <returns>Lista de proyectos</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProjectListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] ProjectStatus? status,
        [FromQuery] string? searchTerm,
        [FromQuery] DateTime? startDateFrom,
        [FromQuery] DateTime? startDateTo)
    {
        var query = new GetProjectsQuery
        {
            Status = status,
            SearchTerm = searchTerm,
            StartDateFrom = startDateFrom,
            StartDateTo = startDateTo
        };

        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene un proyecto por su ID
    /// </summary>
    /// <param name="id">ID del proyecto</param>
    /// <returns>Datos del proyecto</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var query = new GetProjectByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return NotFound(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Crea un nuevo proyecto
    /// </summary>
    /// <param name="command">Datos del proyecto a crear</param>
    /// <returns>ID del proyecto creado</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProjectCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value },
            result.Value);
    }

    /// <summary>
    /// Obtiene el código del proyecto por ID
    /// </summary>
    /// <param name="id">ID del proyecto</param>
    /// <returns>Código del proyecto</returns>
    [HttpGet("{id:guid}/code")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectCode(Guid id)
    {
        var query = new GetProjectByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return NotFound(new { error = result.Error });

        return Ok(new { code = result.Value.Code });
    }
}

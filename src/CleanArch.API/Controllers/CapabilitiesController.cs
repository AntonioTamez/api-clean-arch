using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

/// <summary>
/// Controller para gestión de capacidades funcionales
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CapabilitiesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CapabilitiesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todas las capacidades de una aplicación
    /// </summary>
    /// <param name="applicationId">ID de la aplicación</param>
    /// <returns>Lista de capacidades</returns>
    [HttpGet("application/{applicationId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByApplication(Guid applicationId)
    {
        // TODO: Implementar GetCapabilitiesByApplicationQuery
        return Ok(new List<object>());
    }

    /// <summary>
    /// Obtiene una capacidad por su ID con sus reglas de negocio
    /// </summary>
    /// <param name="id">ID de la capacidad</param>
    /// <returns>Capacidad con reglas de negocio</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        // TODO: Implementar GetCapabilityWithRulesQuery
        return Ok(new { id, name = "Sample Capability" });
    }

    /// <summary>
    /// Crea una nueva capacidad
    /// </summary>
    /// <param name="command">Datos de la capacidad</param>
    /// <returns>ID de la capacidad creada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] object command)
    {
        // TODO: Implementar CreateCapabilityCommand
        var id = Guid.NewGuid();
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Actualiza el estado de una capacidad
    /// </summary>
    /// <param name="id">ID de la capacidad</param>
    /// <param name="status">Nuevo estado</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        // TODO: Implementar ChangeCapabilityStatusCommand
        return NoContent();
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

/// <summary>
/// Controller para gestión de reglas de negocio
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BusinessRulesController : ControllerBase
{
    private readonly IMediator _mediator;

    public BusinessRulesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene todas las reglas de negocio de una capacidad
    /// </summary>
    /// <param name="capabilityId">ID de la capacidad</param>
    /// <returns>Lista de reglas de negocio</returns>
    [HttpGet("capability/{capabilityId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCapability(Guid capabilityId)
    {
        // TODO: Implementar GetBusinessRulesByCapabilityQuery
        return Ok(new List<object>());
    }

    /// <summary>
    /// Obtiene una regla de negocio por su ID
    /// </summary>
    /// <param name="id">ID de la regla</param>
    /// <returns>Regla de negocio</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        // TODO: Implementar GetBusinessRuleByIdQuery
        return Ok(new { id, name = "Sample Business Rule" });
    }

    /// <summary>
    /// Busca reglas de negocio por término
    /// </summary>
    /// <param name="searchTerm">Término de búsqueda</param>
    /// <returns>Lista de reglas encontradas</returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] string searchTerm)
    {
        // TODO: Implementar SearchBusinessRulesQuery
        return Ok(new List<object>());
    }

    /// <summary>
    /// Crea una nueva regla de negocio
    /// </summary>
    /// <param name="command">Datos de la regla</param>
    /// <returns>ID de la regla creada</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] object command)
    {
        // TODO: Implementar CreateBusinessRuleCommand
        var id = Guid.NewGuid();
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Activa una regla de negocio
    /// </summary>
    /// <param name="id">ID de la regla</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id)
    {
        // TODO: Implementar ActivateBusinessRuleCommand
        return NoContent();
    }

    /// <summary>
    /// Desactiva una regla de negocio
    /// </summary>
    /// <param name="id">ID de la regla</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        // TODO: Implementar DeactivateBusinessRuleCommand
        return NoContent();
    }
}

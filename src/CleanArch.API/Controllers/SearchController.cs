using CleanArch.Application.Search.DTOs;
using CleanArch.Application.Search.Queries.GlobalSearch;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

/// <summary>
/// Controller para búsqueda global
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Búsqueda global en todas las entidades
    /// </summary>
    /// <param name="q">Término de búsqueda</param>
    /// <param name="limit">Límite de resultados por tipo (default: 5)</param>
    /// <returns>Resultados de búsqueda agrupados por tipo</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GlobalSearchResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GlobalSearch([FromQuery] string q, [FromQuery] int limit = 5)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Search term is required" });

        var query = new GlobalSearchQuery(q, limit);
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }
}

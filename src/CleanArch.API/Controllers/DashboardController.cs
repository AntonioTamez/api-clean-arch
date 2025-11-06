using CleanArch.Application.Dashboard.DTOs;
using CleanArch.Application.Dashboard.Queries.GetDashboardStats;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

/// <summary>
/// Controller para dashboard y estadísticas
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtiene estadísticas generales del sistema
    /// </summary>
    /// <returns>Estadísticas del dashboard</returns>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStats()
    {
        var query = new GetDashboardStatsQuery();
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    /// <summary>
    /// Obtiene resumen rápido del sistema
    /// </summary>
    /// <returns>Contadores principales</returns>
    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummary()
    {
        var query = new GetDashboardStatsQuery();
        var result = await _mediator.Send(query);

        if (result.IsFailure)
            return BadRequest(new { error = result.Error });

        var summary = new
        {
            totalProjects = result.Value.TotalProjects,
            activeProjects = result.Value.ActiveProjects,
            totalCapabilities = result.Value.TotalCapabilities,
            totalBusinessRules = result.Value.TotalBusinessRules,
            publishedWikiPages = result.Value.PublishedWikiPages
        };

        return Ok(summary);
    }
}

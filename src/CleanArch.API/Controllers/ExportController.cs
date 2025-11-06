using CleanArch.Application.Export.Queries.ExportCapabilities;
using CleanArch.Application.Export.Queries.ExportDashboard;
using CleanArch.Application.Export.Queries.ExportProjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

/// <summary>
/// Controller para exportación de datos a Excel
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ExportController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Exporta todos los proyectos a Excel
    /// </summary>
    /// <returns>Archivo Excel con los proyectos</returns>
    [HttpGet("projects")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportProjects()
    {
        var query = new ExportProjectsQuery();
        var fileBytes = await _mediator.Send(query);

        var fileName = $"Proyectos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    /// <summary>
    /// Exporta las estadísticas del dashboard a Excel (múltiples hojas)
    /// </summary>
    /// <returns>Archivo Excel con estadísticas del dashboard</returns>
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportDashboard()
    {
        var query = new ExportDashboardQuery();
        var fileBytes = await _mediator.Send(query);

        var fileName = $"Dashboard_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    /// <summary>
    /// Exporta todas las capacidades a Excel
    /// </summary>
    /// <returns>Archivo Excel con las capacidades</returns>
    [HttpGet("capabilities")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> ExportCapabilities()
    {
        var query = new ExportCapabilitiesQuery();
        var fileBytes = await _mediator.Send(query);

        var fileName = $"Capacidades_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    /// <summary>
    /// Exporta un reporte completo con todas las entidades (múltiples hojas)
    /// </summary>
    /// <returns>Archivo Excel con reporte completo</returns>
    [HttpGet("full-report")]
    [Authorize] // Requiere autenticación
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ExportFullReport()
    {
        // Obtener datos de dashboard que incluye múltiples hojas
        var query = new ExportDashboardQuery();
        var fileBytes = await _mediator.Send(query);

        var fileName = $"Reporte_Completo_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        
        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }
}

using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Dashboard.Queries.GetDashboardStats;
using MediatR;

namespace CleanArch.Application.Export.Queries.ExportDashboard;

public class ExportDashboardQueryHandler : IRequestHandler<ExportDashboardQuery, byte[]>
{
    private readonly IMediator _mediator;
    private readonly IExcelExportService _excelExportService;

    public ExportDashboardQueryHandler(
        IMediator mediator,
        IExcelExportService excelExportService)
    {
        _mediator = mediator;
        _excelExportService = excelExportService;
    }

    public async Task<byte[]> Handle(ExportDashboardQuery request, CancellationToken cancellationToken)
    {
        // Obtener estadísticas del dashboard
        var statsQuery = new GetDashboardStatsQuery();
        var statsResult = await _mediator.Send(statsQuery, cancellationToken);

        if (statsResult.IsFailure)
            throw new InvalidOperationException("No se pudieron obtener las estadísticas");

        var stats = statsResult.Value;

        // Crear múltiples hojas para el reporte
        var sheets = new Dictionary<string, object>
        {
            // Hoja 1: Resumen general
            ["Resumen General"] = new List<object>
            {
                new { Métrica = "Total Proyectos", Valor = stats.TotalProjects },
                new { Métrica = "Proyectos Activos", Valor = stats.ActiveProjects },
                new { Métrica = "Proyectos Completados", Valor = stats.CompletedProjects },
                new { Métrica = "Total Aplicaciones", Valor = stats.TotalApplications },
                new { Métrica = "Total Capacidades", Valor = stats.TotalCapabilities },
                new { Métrica = "Total Reglas de Negocio", Valor = stats.TotalBusinessRules },
                new { Métrica = "Total Páginas Wiki", Valor = stats.TotalWikiPages },
                new { Métrica = "Páginas Wiki Publicadas", Valor = stats.PublishedWikiPages }
            },

            // Hoja 2: Proyectos por estado
            ["Proyectos por Estado"] = new List<object>
            {
                new { Estado = "Planning", Cantidad = stats.ProjectsByStatus.Planning },
                new { Estado = "In Progress", Cantidad = stats.ProjectsByStatus.InProgress },
                new { Estado = "On Hold", Cantidad = stats.ProjectsByStatus.OnHold },
                new { Estado = "Completed", Cantidad = stats.ProjectsByStatus.Completed },
                new { Estado = "Cancelled", Cantidad = stats.ProjectsByStatus.Cancelled }
            },

            // Hoja 3: Proyectos recientes
            ["Proyectos Recientes"] = stats.RecentProjects.Select(p => new
            {
                Código = p.Code,
                Nombre = p.Name,
                Estado = p.Status,
                Aplicaciones = p.ApplicationsCount,
                Capacidades = p.CapabilitiesCount,
                FechaInicio = p.StartDate,
                FechaFinPlanificada = p.PlannedEndDate
            }).ToList(),

            // Hoja 4: Top capacidades
            ["Top Capacidades"] = stats.TopCapabilities.Select(c => new
            {
                Capacidad = c.Name,
                Aplicación = c.ApplicationName,
                ReglasNegocio = c.BusinessRulesCount,
                Estado = c.Status,
                Prioridad = c.Priority
            }).ToList()
        };

        return _excelExportService.ExportMultipleSheetsToExcel(sheets);
    }
}

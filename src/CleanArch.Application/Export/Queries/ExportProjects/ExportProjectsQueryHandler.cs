using CleanArch.Application.Common.Interfaces;
using CleanArch.Domain.Interfaces;
using MediatR;

namespace CleanArch.Application.Export.Queries.ExportProjects;

public class ExportProjectsQueryHandler : IRequestHandler<ExportProjectsQuery, byte[]>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IExcelExportService _excelExportService;

    public ExportProjectsQueryHandler(
        IProjectRepository projectRepository,
        IExcelExportService excelExportService)
    {
        _projectRepository = projectRepository;
        _excelExportService = excelExportService;
    }

    public async Task<byte[]> Handle(ExportProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetAllAsync(cancellationToken);

        var exportData = projects.Select(p => new
        {
            Código = p.Code.Value,
            Nombre = p.Name,
            Descripción = p.Description,
            Estado = p.Status.ToString(),
            FechaInicio = p.StartDate,
            FechaFinPlanificada = p.PlannedEndDate,
            FechaFinReal = p.ActualEndDate,
            GerenteProyecto = p.ProjectManager,
            Aplicaciones = p.Applications.Count,
            FechaCreación = p.CreatedAt
        }).ToList();

        return _excelExportService.ExportToExcel(exportData, "Proyectos");
    }
}

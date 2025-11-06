using CleanArch.Application.Common.Interfaces;
using CleanArch.Domain.Interfaces;
using MediatR;

namespace CleanArch.Application.Export.Queries.ExportCapabilities;

public class ExportCapabilitiesQueryHandler : IRequestHandler<ExportCapabilitiesQuery, byte[]>
{
    private readonly ICapabilityRepository _capabilityRepository;
    private readonly IExcelExportService _excelExportService;

    public ExportCapabilitiesQueryHandler(
        ICapabilityRepository capabilityRepository,
        IExcelExportService excelExportService)
    {
        _capabilityRepository = capabilityRepository;
        _excelExportService = excelExportService;
    }

    public async Task<byte[]> Handle(ExportCapabilitiesQuery request, CancellationToken cancellationToken)
    {
        var capabilities = await _capabilityRepository.GetAllAsync(cancellationToken);

        var exportData = capabilities.Select(c => new
        {
            Nombre = c.Name,
            Descripción = c.Description,
            Estado = c.Status.ToString(),
            Categoría = c.Category.ToString(),
            Prioridad = c.Priority,
            ReglasNegocio = c.BusinessRules.Count,
            FechaCreación = c.CreatedAt
        }).ToList();

        return _excelExportService.ExportToExcel(exportData, "Capacidades");
    }
}

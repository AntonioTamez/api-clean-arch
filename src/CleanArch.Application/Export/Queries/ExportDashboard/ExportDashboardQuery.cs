using MediatR;

namespace CleanArch.Application.Export.Queries.ExportDashboard;

/// <summary>
/// Query para exportar estadísticas del dashboard a Excel
/// </summary>
public record ExportDashboardQuery : IRequest<byte[]>;

using MediatR;

namespace CleanArch.Application.Export.Queries.ExportCapabilities;

/// <summary>
/// Query para exportar capacidades a Excel
/// </summary>
public record ExportCapabilitiesQuery : IRequest<byte[]>;

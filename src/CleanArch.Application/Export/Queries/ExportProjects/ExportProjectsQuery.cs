using MediatR;

namespace CleanArch.Application.Export.Queries.ExportProjects;

/// <summary>
/// Query para exportar proyectos a Excel
/// </summary>
public record ExportProjectsQuery : IRequest<byte[]>;

using CleanArch.Application.Common.Models;
using CleanArch.Application.Projects.DTOs;
using CleanArch.Domain.Enums;
using MediatR;

namespace CleanArch.Application.Projects.Queries.GetProjects;

/// <summary>
/// Query para obtener lista de proyectos con filtros opcionales
/// </summary>
public class GetProjectsQuery : IRequest<Result<List<ProjectListItemDto>>>
{
    public ProjectStatus? Status { get; init; }
    public string? SearchTerm { get; init; }
    public DateTime? StartDateFrom { get; init; }
    public DateTime? StartDateTo { get; init; }
}

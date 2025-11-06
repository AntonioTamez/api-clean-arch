using CleanArch.Application.Common.Models;
using CleanArch.Application.Projects.DTOs;
using MediatR;

namespace CleanArch.Application.Projects.Queries.GetProjectById;

/// <summary>
/// Query para obtener un proyecto por su ID
/// </summary>
public record GetProjectByIdQuery(Guid ProjectId) : IRequest<Result<ProjectDto>>;

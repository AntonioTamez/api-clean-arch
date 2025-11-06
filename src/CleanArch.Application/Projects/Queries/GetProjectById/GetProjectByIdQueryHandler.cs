using AutoMapper;
using CleanArch.Application.Common.Models;
using CleanArch.Application.Projects.DTOs;
using CleanArch.Domain.Interfaces;
using MediatR;

namespace CleanArch.Application.Projects.Queries.GetProjectById;

/// <summary>
/// Handler para obtener un proyecto por ID
/// </summary>
public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, Result<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public GetProjectByIdQueryHandler(
        IProjectRepository projectRepository,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<Result<ProjectDto>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);

        if (project == null)
            return Result<ProjectDto>.Failure(new Error("Project.NotFound", $"Project with ID '{request.ProjectId}' was not found"));

        var projectDto = _mapper.Map<ProjectDto>(project);

        return Result<ProjectDto>.Success(projectDto);
    }
}

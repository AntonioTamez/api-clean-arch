using AutoMapper;
using CleanArch.Application.Common.Models;
using CleanArch.Application.Projects.DTOs;
using CleanArch.Domain.Interfaces;
using MediatR;

namespace CleanArch.Application.Projects.Queries.GetProjects;

/// <summary>
/// Handler para obtener lista de proyectos
/// </summary>
public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, Result<List<ProjectListItemDto>>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public GetProjectsQueryHandler(
        IProjectRepository projectRepository,
        IMapper mapper)
    {
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task<Result<List<ProjectListItemDto>>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = await _projectRepository.GetAllAsync(cancellationToken);

        // Aplicar filtros
        if (request.Status.HasValue)
            projects = projects.Where(p => p.Status == request.Status.Value).ToList();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            projects = projects.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                p.Code.Value.ToLower().Contains(searchTerm) ||
                p.Description.ToLower().Contains(searchTerm)).ToList();
        }

        if (request.StartDateFrom.HasValue)
            projects = projects.Where(p => p.StartDate >= request.StartDateFrom.Value).ToList();

        if (request.StartDateTo.HasValue)
            projects = projects.Where(p => p.StartDate <= request.StartDateTo.Value).ToList();

        var projectDtos = _mapper.Map<List<ProjectListItemDto>>(projects);

        return Result<List<ProjectListItemDto>>.Success(projectDtos);
    }
}

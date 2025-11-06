using CleanArch.Application.Common.Models;
using CleanArch.Application.Search.DTOs;
using CleanArch.Domain.Interfaces;
using MediatR;

namespace CleanArch.Application.Search.Queries.GlobalSearch;

public class GlobalSearchQueryHandler : IRequestHandler<GlobalSearchQuery, Result<GlobalSearchResultDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ICapabilityRepository _capabilityRepository;
    private readonly IBusinessRuleRepository _businessRuleRepository;
    private readonly IWikiPageRepository _wikiPageRepository;

    public GlobalSearchQueryHandler(
        IProjectRepository projectRepository,
        ICapabilityRepository capabilityRepository,
        IBusinessRuleRepository businessRuleRepository,
        IWikiPageRepository wikiPageRepository)
    {
        _projectRepository = projectRepository;
        _capabilityRepository = capabilityRepository;
        _businessRuleRepository = businessRuleRepository;
        _wikiPageRepository = wikiPageRepository;
    }

    public async Task<Result<GlobalSearchResultDto>> Handle(GlobalSearchQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.SearchTerm))
            return Result<GlobalSearchResultDto>.Failure(new Error("Search.EmptyTerm", "Search term cannot be empty"));

        try
        {
            var searchTermLower = request.SearchTerm.ToLower();

            // Buscar en proyectos
            var allProjects = await _projectRepository.GetAllAsync(cancellationToken);
            var projectResults = allProjects
                .Where(p => p.Name.ToLower().Contains(searchTermLower) ||
                           p.Description.ToLower().Contains(searchTermLower) ||
                           p.Code.Value.ToLower().Contains(searchTermLower))
                .Take(request.MaxResultsPerType)
                .Select(p => new SearchItemDto
                {
                    Id = p.Id,
                    Type = "Project",
                    Title = p.Name,
                    Description = p.Description,
                    Code = p.Code.Value,
                    Status = p.Status.ToString(),
                    CreatedAt = p.CreatedAt
                })
                .ToList();

            // Buscar en capacidades
            var allCapabilities = await _capabilityRepository.GetAllAsync(cancellationToken);
            var capabilityResults = allCapabilities
                .Where(c => c.Name.ToLower().Contains(searchTermLower) ||
                           c.Description.ToLower().Contains(searchTermLower))
                .Take(request.MaxResultsPerType)
                .Select(c => new SearchItemDto
                {
                    Id = c.Id,
                    Type = "Capability",
                    Title = c.Name,
                    Description = c.Description,
                    Code = "",
                    Status = c.Status.ToString(),
                    CreatedAt = c.CreatedAt
                })
                .ToList();

            // Buscar en reglas de negocio
            var businessRuleResults = (await _businessRuleRepository.SearchAsync(request.SearchTerm, cancellationToken))
                .Take(request.MaxResultsPerType)
                .Select(br => new SearchItemDto
                {
                    Id = br.Id,
                    Type = "BusinessRule",
                    Title = br.Name,
                    Description = br.Description,
                    Code = br.Code.Value,
                    Status = br.Status.ToString(),
                    CreatedAt = br.CreatedAt
                })
                .ToList();

            // Buscar en wiki pages
            var wikiResults = (await _wikiPageRepository.SearchAsync(request.SearchTerm, cancellationToken))
                .Take(request.MaxResultsPerType)
                .Select(w => new SearchItemDto
                {
                    Id = w.Id,
                    Type = "WikiPage",
                    Title = w.Title,
                    Description = w.CurrentContent.Length > 200 
                        ? w.CurrentContent.Substring(0, 200) + "..." 
                        : w.CurrentContent,
                    Code = w.Slug.Value,
                    Status = w.IsPublished ? "Published" : "Draft",
                    CreatedAt = w.CreatedAt
                })
                .ToList();

            var result = new GlobalSearchResultDto
            {
                Projects = projectResults,
                Capabilities = capabilityResults,
                BusinessRules = businessRuleResults,
                WikiPages = wikiResults,
                TotalResults = projectResults.Count + capabilityResults.Count + 
                              businessRuleResults.Count + wikiResults.Count
            };

            return Result<GlobalSearchResultDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<GlobalSearchResultDto>.Failure(new Error("Search.Error", ex.Message));
        }
    }
}

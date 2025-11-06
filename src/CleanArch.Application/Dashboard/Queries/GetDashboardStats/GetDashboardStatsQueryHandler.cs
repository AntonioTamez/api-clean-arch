using CleanArch.Application.Common.Interfaces;
using CleanArch.Application.Common.Models;
using CleanArch.Application.Dashboard.DTOs;
using CleanArch.Domain.Enums;
using CleanArch.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArch.Application.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IProjectRepository _projectRepository;
    private readonly ICapabilityRepository _capabilityRepository;
    private readonly IBusinessRuleRepository _businessRuleRepository;
    private readonly IWikiPageRepository _wikiPageRepository;

    public GetDashboardStatsQueryHandler(
        IApplicationDbContext context,
        IProjectRepository projectRepository,
        ICapabilityRepository capabilityRepository,
        IBusinessRuleRepository businessRuleRepository,
        IWikiPageRepository wikiPageRepository)
    {
        _context = context;
        _projectRepository = projectRepository;
        _capabilityRepository = capabilityRepository;
        _businessRuleRepository = businessRuleRepository;
        _wikiPageRepository = wikiPageRepository;
    }

    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Obtener todos los proyectos
            var projects = await _projectRepository.GetAllAsync(cancellationToken);
            var allCapabilities = await _capabilityRepository.GetAllAsync(cancellationToken);
            var allBusinessRules = await _businessRuleRepository.GetAllAsync(cancellationToken);
            var allWikiPages = await _wikiPageRepository.GetAllAsync(cancellationToken);

            var stats = new DashboardStatsDto
            {
                TotalProjects = projects.Count,
                ActiveProjects = projects.Count(p => p.Status == ProjectStatus.InProgress),
                CompletedProjects = projects.Count(p => p.Status == ProjectStatus.Completed),
                TotalApplications = projects.SelectMany(p => p.Applications).Count(),
                TotalCapabilities = allCapabilities.Count,
                TotalBusinessRules = allBusinessRules.Count,
                TotalWikiPages = allWikiPages.Count,
                PublishedWikiPages = allWikiPages.Count(w => w.IsPublished),

                ProjectsByStatus = new ProjectsByStatusDto
                {
                    Planning = projects.Count(p => p.Status == ProjectStatus.Planning),
                    InProgress = projects.Count(p => p.Status == ProjectStatus.InProgress),
                    OnHold = projects.Count(p => p.Status == ProjectStatus.OnHold),
                    Completed = projects.Count(p => p.Status == ProjectStatus.Completed),
                    Cancelled = projects.Count(p => p.Status == ProjectStatus.Cancelled)
                },

                RecentProjects = projects
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(p => new ProjectProgressDto
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Code = p.Code.Value,
                        Status = p.Status.ToString(),
                        ApplicationsCount = p.Applications.Count,
                        CapabilitiesCount = p.Applications.SelectMany(a => a.Capabilities).Count(),
                        StartDate = p.StartDate,
                        PlannedEndDate = p.PlannedEndDate
                    })
                    .ToList(),

                TopCapabilities = allCapabilities
                    .OrderByDescending(c => c.BusinessRules.Count)
                    .ThenBy(c => c.Priority)
                    .Take(10)
                    .Select(c => new TopCapabilityDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ApplicationName = _context.Applications
                            .Where(a => a.Id == c.ApplicationId)
                            .Select(a => a.Name)
                            .FirstOrDefault() ?? "Unknown",
                        BusinessRulesCount = c.BusinessRules.Count,
                        Status = c.Status.ToString(),
                        Priority = (int)c.Priority
                    })
                    .ToList()
            };

            return Result<DashboardStatsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            return Result<DashboardStatsDto>.Failure(new Error("DashboardStats.Error", ex.Message));
        }
    }
}

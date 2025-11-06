namespace CleanArch.Application.Dashboard.DTOs;

/// <summary>
/// Estadísticas generales del dashboard
/// </summary>
public class DashboardStatsDto
{
    public int TotalProjects { get; set; }
    public int ActiveProjects { get; set; }
    public int CompletedProjects { get; set; }
    public int TotalApplications { get; set; }
    public int TotalCapabilities { get; set; }
    public int TotalBusinessRules { get; set; }
    public int TotalWikiPages { get; set; }
    public int PublishedWikiPages { get; set; }
    
    public ProjectsByStatusDto ProjectsByStatus { get; set; } = new();
    public List<ProjectProgressDto> RecentProjects { get; set; } = new();
    public List<TopCapabilityDto> TopCapabilities { get; set; } = new();
}

public class ProjectsByStatusDto
{
    public int Planning { get; set; }
    public int InProgress { get; set; }
    public int OnHold { get; set; }
    public int Completed { get; set; }
    public int Cancelled { get; set; }
}

public class ProjectProgressDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ApplicationsCount { get; set; }
    public int CapabilitiesCount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
}

public class TopCapabilityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = string.Empty;
    public int BusinessRulesCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Priority { get; set; }
}

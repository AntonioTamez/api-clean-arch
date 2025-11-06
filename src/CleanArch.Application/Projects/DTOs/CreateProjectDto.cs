namespace CleanArch.Application.Projects.DTOs;

/// <summary>
/// DTO para crear un nuevo proyecto
/// </summary>
public record CreateProjectDto
{
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime? PlannedEndDate { get; init; }
    public string ProjectManager { get; init; } = null!;
}

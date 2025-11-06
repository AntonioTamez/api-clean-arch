namespace CleanArch.Application.Projects.DTOs;

/// <summary>
/// DTO para representar un proyecto
/// </summary>
public record ProjectDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public string Status { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime? PlannedEndDate { get; init; }
    public DateTime? ActualEndDate { get; init; }
    public string ProjectManager { get; init; } = null!;
    public int ApplicationCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime? ModifiedAt { get; init; }
    public string? ModifiedBy { get; init; }
}

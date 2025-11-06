namespace CleanArch.Application.Projects.DTOs;

/// <summary>
/// DTO simplificado para listados de proyectos
/// </summary>
public record ProjectListItemDto
{
    public Guid Id { get; init; }
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Status { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime? PlannedEndDate { get; init; }
    public string ProjectManager { get; init; } = null!;
    public int ApplicationCount { get; init; }
}

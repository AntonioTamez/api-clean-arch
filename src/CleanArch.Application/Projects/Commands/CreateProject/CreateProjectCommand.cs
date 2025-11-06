using CleanArch.Application.Common.Models;
using MediatR;

namespace CleanArch.Application.Projects.Commands.CreateProject;

/// <summary>
/// Comando para crear un nuevo proyecto
/// </summary>
public class CreateProjectCommand : IRequest<Result<Guid>>
{
    public string Code { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string Description { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime? PlannedEndDate { get; init; }
    public string ProjectManager { get; init; } = null!;
}

using CleanArch.Application.Common.Models;
using CleanArch.Domain.Entities;
using CleanArch.Domain.Interfaces;
using CleanArch.Domain.ValueObjects;
using MediatR;

namespace CleanArch.Application.Projects.Commands.CreateProject;

/// <summary>
/// Handler para el comando CreateProject
/// </summary>
public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Result<Guid>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        // Validar que no exista un proyecto con el mismo código
        var existingProject = await _projectRepository.GetByCodeAsync(request.Code, cancellationToken);
        if (existingProject != null)
            return Result<Guid>.Failure(new Error("Project.DuplicateCode", $"A project with code '{request.Code}' already exists"));

        // Crear ProjectCode
        var codeResult = ProjectCode.Create(request.Code);
        if (codeResult.IsFailure)
            return Result<Guid>.Failure(new Error("Project.InvalidCode", codeResult.Error));

        // Crear Project
        var projectResult = Project.Create(
            codeResult.Value,
            request.Name,
            request.Description,
            request.StartDate,
            request.ProjectManager);

        if (projectResult.IsFailure)
            return Result<Guid>.Failure(new Error("Project.CreationFailed", projectResult.Error));

        var project = projectResult.Value;

        // Si hay PlannedEndDate, establecerla
        if (request.PlannedEndDate.HasValue)
        {
            var endDateResult = project.SetPlannedEndDate(request.PlannedEndDate.Value);
            if (endDateResult.IsFailure)
                return Result<Guid>.Failure(new Error("Project.InvalidEndDate", endDateResult.Error));
        }

        // Guardar
        await _projectRepository.AddAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(project.Id);
    }
}

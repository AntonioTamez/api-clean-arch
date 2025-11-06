using FluentValidation;

namespace CleanArch.Application.Projects.Commands.CreateProject;

/// <summary>
/// Validador para CreateProjectCommand
/// </summary>
public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Project code is required")
            .MinimumLength(3).WithMessage("Project code must be at least 3 characters")
            .MaximumLength(30).WithMessage("Project code cannot exceed 30 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(200).WithMessage("Project name cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Project description is required")
            .MaximumLength(2000).WithMessage("Project description cannot exceed 2000 characters");

        RuleFor(x => x.ProjectManager)
            .NotEmpty().WithMessage("Project manager is required")
            .MaximumLength(100).WithMessage("Project manager name cannot exceed 100 characters");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required");

        RuleFor(x => x.PlannedEndDate)
            .GreaterThan(x => x.StartDate)
            .When(x => x.PlannedEndDate.HasValue)
            .WithMessage("Planned end date must be after start date");
    }
}

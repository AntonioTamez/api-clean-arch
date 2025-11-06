using CleanArch.Domain.Common;

namespace CleanArch.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un código único de proyecto
/// Formato: Mayúsculas, 3-30 caracteres, permite letras, números, guiones y puntos
/// Ejemplos: PRJ-2024-001, BACKEND-API, MOBILE_APP
/// </summary>
public sealed class ProjectCode : ValueObject
{
    public string Value { get; }

    private ProjectCode(string value)
    {
        Value = value;
    }

    public static Result<ProjectCode> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result<ProjectCode>.Failure("Project code cannot be empty");

        code = code.Trim().ToUpper();

        if (code.Length < 3)
            return Result<ProjectCode>.Failure("Project code must have at least 3 characters");

        if (code.Length > 30)
            return Result<ProjectCode>.Failure("Project code length cannot exceed 30 characters");

        if (!IsValidFormat(code))
            return Result<ProjectCode>.Failure("Project code can only contain letters, numbers, hyphens, underscores and dots");

        return Result<ProjectCode>.Success(new ProjectCode(code));
    }

    private static bool IsValidFormat(string code)
    {
        // Solo permite letras, números, guiones, guiones bajos y puntos
        return code.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_' || c == '.');
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    // Implicit conversion para facilitar uso
    public static implicit operator string(ProjectCode code) => code.Value;
}

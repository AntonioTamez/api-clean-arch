using CleanArch.Domain.Common;

namespace CleanArch.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un código único de regla de negocio
/// Formato: BR-XXX-NNN (BR=BusinessRule, XXX=Categoría, NNN=Número)
/// Ejemplos: BR-VAL-001, BR-CALC-045, BR-AUTH-100
/// </summary>
public sealed class RuleCode : ValueObject
{
    public string Value { get; }

    private RuleCode(string value)
    {
        Value = value;
    }

    public static Result<RuleCode> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return Result<RuleCode>.Failure("Rule code cannot be empty");

        code = code.Trim().ToUpper();

        if (code.Length < 5)
            return Result<RuleCode>.Failure("Rule code must have at least 5 characters");

        if (code.Length > 20)
            return Result<RuleCode>.Failure("Rule code length cannot exceed 20 characters");

        if (!IsValidFormat(code))
            return Result<RuleCode>.Failure("Rule code can only contain letters, numbers, and hyphens");

        return Result<RuleCode>.Success(new RuleCode(code));
    }

    private static bool IsValidFormat(string code)
    {
        // Permite letras, números y guiones
        return code.All(c => char.IsLetterOrDigit(c) || c == '-');
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(RuleCode code) => code.Value;
}

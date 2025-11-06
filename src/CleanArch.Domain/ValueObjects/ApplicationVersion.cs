using CleanArch.Domain.Common;

namespace CleanArch.Domain.ValueObjects;

/// <summary>
/// Value Object que representa la versión de una aplicación usando Semantic Versioning (SemVer)
/// Formato: MAJOR.MINOR.PATCH
/// Ejemplos: 1.0.0, 2.5.3, 10.0.1
/// </summary>
public sealed class ApplicationVersion : ValueObject, IComparable<ApplicationVersion>
{
    public int Major { get; }
    public int Minor { get; }
    public int Patch { get; }

    private ApplicationVersion(int major, int minor, int patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }

    public static Result<ApplicationVersion> Create(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return Result<ApplicationVersion>.Failure("Version cannot be empty");

        var parts = version.Trim().Split('.');

        if (parts.Length != 3)
            return Result<ApplicationVersion>.Failure("Version must follow SemVer format: MAJOR.MINOR.PATCH");

        if (!int.TryParse(parts[0], out var major) || major < 0)
            return Result<ApplicationVersion>.Failure("Major version must be a non-negative integer");

        if (!int.TryParse(parts[1], out var minor) || minor < 0)
            return Result<ApplicationVersion>.Failure("Minor version must be a non-negative integer");

        if (!int.TryParse(parts[2], out var patch) || patch < 0)
            return Result<ApplicationVersion>.Failure("Patch version must be a non-negative integer");

        return Result<ApplicationVersion>.Success(new ApplicationVersion(major, minor, patch));
    }

    public ApplicationVersion IncrementMajor() => new(Major + 1, 0, 0);
    public ApplicationVersion IncrementMinor() => new(Major, Minor + 1, 0);
    public ApplicationVersion IncrementPatch() => new(Major, Minor, Patch + 1);

    public int CompareTo(ApplicationVersion? other)
    {
        if (other is null) return 1;

        var majorComparison = Major.CompareTo(other.Major);
        if (majorComparison != 0) return majorComparison;

        var minorComparison = Minor.CompareTo(other.Minor);
        if (minorComparison != 0) return minorComparison;

        return Patch.CompareTo(other.Patch);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Major;
        yield return Minor;
        yield return Patch;
    }

    public override string ToString() => $"{Major}.{Minor}.{Patch}";

    // Operators
    public static bool operator >(ApplicationVersion left, ApplicationVersion right) => left.CompareTo(right) > 0;
    public static bool operator <(ApplicationVersion left, ApplicationVersion right) => left.CompareTo(right) < 0;
    public static bool operator >=(ApplicationVersion left, ApplicationVersion right) => left.CompareTo(right) >= 0;
    public static bool operator <=(ApplicationVersion left, ApplicationVersion right) => left.CompareTo(right) <= 0;
}

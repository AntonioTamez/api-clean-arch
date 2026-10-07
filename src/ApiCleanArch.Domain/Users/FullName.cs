namespace ApiCleanArch.Domain.Users;

public sealed record FullName
{
    public const int MinLength = 2;
    public const int MaxLength = 100;

    private FullName(string value) => Value = value;

    public string Value { get; }

    public static FullName Create(string? raw)
    {
        var value = raw?.Trim() ?? string.Empty;

        if (value.Length is < MinLength or > MaxLength)
        {
            throw new InvalidFullNameException(
                $"The full name must be between {MinLength} and {MaxLength} characters.");
        }

        return new FullName(value);
    }

    public override string ToString() => Value;
}

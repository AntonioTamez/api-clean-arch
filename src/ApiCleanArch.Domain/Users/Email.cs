namespace ApiCleanArch.Domain.Users;

public sealed record Email
{
    public const int MaxLength = 254;

    private Email(string value) => Value = value;

    public string Value { get; }

    public static Email Create(string? raw)
    {
        var value = raw?.Trim().ToLowerInvariant() ?? string.Empty;

        if (value.Length == 0)
        {
            throw new InvalidEmailException("The email is required.");
        }

        if (value.Length > MaxLength)
        {
            throw new InvalidEmailException($"The email must not exceed {MaxLength} characters.");
        }

        if (!HasValidShape(value))
        {
            throw new InvalidEmailException("The email format is invalid.");
        }

        return new Email(value);
    }

    public override string ToString() => Value;

    private static bool HasValidShape(string value)
    {
        if (value.Any(char.IsWhiteSpace))
        {
            return false;
        }

        var parts = value.Split('@');
        if (parts.Length != 2)
        {
            return false;
        }

        var (local, domain) = (parts[0], parts[1]);

        return local.Length > 0
            && domain.Contains('.')
            && !domain.StartsWith('.')
            && !domain.EndsWith('.')
            && !domain.Contains("..");
    }
}

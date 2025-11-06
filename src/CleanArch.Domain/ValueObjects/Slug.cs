using CleanArch.Domain.Common;
using System.Text.RegularExpressions;

namespace CleanArch.Domain.ValueObjects;

/// <summary>
/// Value Object que representa un slug URL-friendly
/// Formato: lowercase, hyphens, alphanumeric
/// Ejemplos: user-authentication, payment-gateway, api-integration-guide
/// </summary>
public sealed class Slug : ValueObject
{
    public string Value { get; }

    private Slug(string value)
    {
        Value = value;
    }

    public static Result<Slug> Create(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result<Slug>.Failure("Slug cannot be empty");

        var slug = GenerateSlug(text);

        if (slug.Length < 3)
            return Result<Slug>.Failure("Slug must have at least 3 characters");

        if (slug.Length > 100)
            return Result<Slug>.Failure("Slug cannot exceed 100 characters");

        return Result<Slug>.Success(new Slug(slug));
    }

    private static string GenerateSlug(string text)
    {
        // Convertir a lowercase
        var slug = text.ToLowerInvariant().Trim();

        // Reemplazar espacios y caracteres especiales con guiones
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");

        // Remover guiones del inicio y fin
        slug = slug.Trim('-');

        return slug;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    public static implicit operator string(Slug slug) => slug.Value;
}

namespace CleanArch.Domain.Exceptions;

/// <summary>
/// Excepción base para todas las excepciones del dominio
/// </summary>
public class DomainException : Exception
{
    public DomainException()
    {
    }

    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

/// <summary>
/// Excepción cuando una entidad no se encuentra
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}

/// <summary>
/// Excepción de validación del dominio
/// </summary>
public class ValidationException : DomainException
{
    public ValidationException(string message)
        : base(message)
    {
    }

    public ValidationException(string message, IDictionary<string, string[]> errors)
        : base(message)
    {
        Errors = errors;
    }

    public IDictionary<string, string[]>? Errors { get; }
}

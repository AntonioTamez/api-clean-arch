namespace CleanArch.Domain.Enums;

/// <summary>
/// Prioridad de una capability o regla de negocio
/// </summary>
public enum Priority
{
    /// <summary>
    /// Prioridad baja
    /// </summary>
    Low = 1,

    /// <summary>
    /// Prioridad media
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Prioridad alta
    /// </summary>
    High = 3,

    /// <summary>
    /// Prioridad crítica
    /// </summary>
    Critical = 4
}

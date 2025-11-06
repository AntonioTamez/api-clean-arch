namespace CleanArch.Domain.Enums;

/// <summary>
/// Estados posibles de una capacidad
/// </summary>
public enum CapabilityStatus
{
    /// <summary>
    /// Capacidad planeada
    /// </summary>
    Planned = 1,

    /// <summary>
    /// Capacidad en progreso
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Capacidad completada
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Capacidad deprecada
    /// </summary>
    Deprecated = 4
}

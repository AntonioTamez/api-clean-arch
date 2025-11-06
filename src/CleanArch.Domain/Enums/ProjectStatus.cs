namespace CleanArch.Domain.Enums;

/// <summary>
/// Estados posibles de un proyecto
/// </summary>
public enum ProjectStatus
{
    /// <summary>
    /// Proyecto en fase de planeación
    /// </summary>
    Planning = 1,

    /// <summary>
    /// Proyecto en progreso
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Proyecto en pausa
    /// </summary>
    OnHold = 3,

    /// <summary>
    /// Proyecto completado
    /// </summary>
    Completed = 4,

    /// <summary>
    /// Proyecto cancelado
    /// </summary>
    Cancelled = 5
}

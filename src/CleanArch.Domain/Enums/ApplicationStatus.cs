namespace CleanArch.Domain.Enums;

/// <summary>
/// Estados posibles de una aplicación
/// </summary>
public enum ApplicationStatus
{
    /// <summary>
    /// Aplicación en fase de planeación
    /// </summary>
    Planning = 1,

    /// <summary>
    /// Aplicación en desarrollo
    /// </summary>
    Development = 2,

    /// <summary>
    /// Aplicación en pruebas
    /// </summary>
    Testing = 3,

    /// <summary>
    /// Aplicación en producción
    /// </summary>
    Production = 4,

    /// <summary>
    /// Aplicación descontinuada
    /// </summary>
    Deprecated = 5
}

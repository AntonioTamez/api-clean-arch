namespace CleanArch.Domain.Enums;

/// <summary>
/// Tipo de aplicación en el proyecto
/// </summary>
public enum ApplicationType
{
    /// <summary>
    /// Aplicación nueva
    /// </summary>
    New = 1,

    /// <summary>
    /// Aplicación existente que será modificada
    /// </summary>
    Modified = 2,

    /// <summary>
    /// Aplicación heredada/legacy
    /// </summary>
    Legacy = 3
}

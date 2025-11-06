namespace CleanArch.Domain.Enums;

/// <summary>
/// Tipo de entidad a la que está asociada una página wiki
/// </summary>
public enum WikiEntityType
{
    /// <summary>
    /// Página general sin asociación
    /// </summary>
    General = 1,

    /// <summary>
    /// Asociada a un proyecto
    /// </summary>
    Project = 2,

    /// <summary>
    /// Asociada a una aplicación
    /// </summary>
    Application = 3,

    /// <summary>
    /// Asociada a una capacidad
    /// </summary>
    Capability = 4
}

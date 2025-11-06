namespace CleanArch.Domain.Enums;

/// <summary>
/// Estados de una regla de negocio
/// </summary>
public enum BusinessRuleStatus
{
    /// <summary>
    /// Regla activa
    /// </summary>
    Active = 1,

    /// <summary>
    /// Regla inactiva
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Regla deprecada
    /// </summary>
    Deprecated = 3
}

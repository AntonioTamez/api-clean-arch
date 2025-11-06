namespace CleanArch.Domain.Enums;

/// <summary>
/// Tipos de reglas de negocio
/// </summary>
public enum BusinessRuleType
{
    /// <summary>
    /// Regla de validación
    /// </summary>
    Validation = 1,

    /// <summary>
    /// Regla de cálculo
    /// </summary>
    Calculation = 2,

    /// <summary>
    /// Regla de autorización
    /// </summary>
    Authorization = 3,

    /// <summary>
    /// Regla de flujo de trabajo
    /// </summary>
    Workflow = 4,

    /// <summary>
    /// Regla de transformación de datos
    /// </summary>
    DataTransformation = 5
}

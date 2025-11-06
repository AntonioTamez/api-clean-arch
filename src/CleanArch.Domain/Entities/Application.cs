using CleanArch.Domain.Common;

namespace CleanArch.Domain.Entities;

/// <summary>
/// Agregado raíz que representa una aplicación dentro de un proyecto
/// (Implementación temporal - se completará en siguiente fase)
/// </summary>
public class Application : BaseAuditableEntity
{
    private Application() { } // EF Core

    public string Name { get; private set; } = null!;
    public Guid ProjectId { get; private set; }

    // TODO: Implementar completamente en siguiente fase
}

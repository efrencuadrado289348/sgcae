namespace SGCAE.Domain.Common;

/// <summary>Base de toda entidad: identidad propia que no cambia en su ciclo de vida.</summary>
public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();
}

/// <summary>
/// Raíz de agregado: único punto de entrada para modificar el agregado.
/// Solo las raíces tienen repositorio.
/// </summary>
public abstract class AggregateRoot : Entity;

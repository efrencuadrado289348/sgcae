namespace SGCAE.Application.Abstractions;

/// <summary>Confirma en una sola transacción todos los cambios hechos por los repositorios.</summary>
public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct);
}

namespace SGCAE.Application.Abstractions;

// CQRS: un COMANDO cambia el estado (escribe); una CONSULTA solo lee y nunca modifica.

public interface ICommand<TResult>;

public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    Task<TResult> HandleAsync(TCommand command, CancellationToken ct);
}

public interface IQuery<TResult>;

public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken ct);
}

/// <summary>Resultado vacío para comandos que no devuelven datos.</summary>
public readonly record struct Unit
{
    public static readonly Unit Value = new();
}

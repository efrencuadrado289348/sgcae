using System.Reflection;
using System.Runtime.ExceptionServices;

namespace SGCAE.Application.Abstractions;

/// <summary>Envía cada comando/consulta a su único handler. Desacopla la API de los casos de uso.</summary>
public interface IDispatcher
{
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken ct = default);
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default);
}

internal sealed class Dispatcher(IServiceProvider provider) : IDispatcher
{
    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken ct = default) =>
        Invoke<TResult>(typeof(ICommandHandler<,>), command, ct);

    public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken ct = default) =>
        Invoke<TResult>(typeof(IQueryHandler<,>), query, ct);

    private Task<TResult> Invoke<TResult>(Type openHandlerType, object request, CancellationToken ct)
    {
        var handlerType = openHandlerType.MakeGenericType(request.GetType(), typeof(TResult));
        var handler = provider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No hay handler registrado para {request.GetType().Name}.");

        try
        {
            return (Task<TResult>)handlerType.GetMethod("HandleAsync")!.Invoke(handler, [request, ct])!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            ExceptionDispatchInfo.Throw(ex.InnerException); // conserva la excepción original
            throw; // inalcanzable
        }
    }
}

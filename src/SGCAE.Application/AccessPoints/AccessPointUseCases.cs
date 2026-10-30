using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;
using SGCAE.Domain.AccessPoints;

namespace SGCAE.Application.AccessPoints;

public sealed record RegisterAccessPointCommand(string Code, string Name, AccessPointType Type) : ICommand<Guid>;
public sealed record SetAccessPointEnabledCommand(Guid Id, bool Enabled) : ICommand<Unit>;
public sealed record GetAccessPointByIdQuery(Guid Id) : IQuery<AccessPointDto>;
public sealed record ListAccessPointsQuery(bool? IsEnabled) : IQuery<IReadOnlyList<AccessPointDto>>;

internal sealed class RegisterAccessPointHandler(IAccessPointRepository points, IUnitOfWork uow)
    : ICommandHandler<RegisterAccessPointCommand, Guid>
{
    public async Task<Guid> HandleAsync(RegisterAccessPointCommand cmd, CancellationToken ct)
    {
        var accessPoint = AccessPoint.Create(cmd.Code, cmd.Name, cmd.Type);

        if (await points.ExistsByCodeAsync(accessPoint.Code, ct))
            throw new ConflictException($"Ya existe un punto de acceso con el código {accessPoint.Code}.");

        points.Add(accessPoint);
        await uow.SaveChangesAsync(ct);
        return accessPoint.Id;
    }
}

internal sealed class SetAccessPointEnabledHandler(IAccessPointRepository points, IUnitOfWork uow)
    : ICommandHandler<SetAccessPointEnabledCommand, Unit>
{
    public async Task<Unit> HandleAsync(SetAccessPointEnabledCommand cmd, CancellationToken ct)
    {
        var accessPoint = await points.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException("Punto de acceso", cmd.Id);

        if (cmd.Enabled) accessPoint.Enable();
        else accessPoint.Disable();

        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

internal sealed class GetAccessPointByIdHandler(IAccessPointQueries queries)
    : IQueryHandler<GetAccessPointByIdQuery, AccessPointDto>
{
    public async Task<AccessPointDto> HandleAsync(GetAccessPointByIdQuery query, CancellationToken ct) =>
        await queries.GetByIdAsync(query.Id, ct) ?? throw new NotFoundException("Punto de acceso", query.Id);
}

internal sealed class ListAccessPointsHandler(IAccessPointQueries queries)
    : IQueryHandler<ListAccessPointsQuery, IReadOnlyList<AccessPointDto>>
{
    public Task<IReadOnlyList<AccessPointDto>> HandleAsync(ListAccessPointsQuery query, CancellationToken ct) =>
        queries.ListAsync(query.IsEnabled, ct);
}

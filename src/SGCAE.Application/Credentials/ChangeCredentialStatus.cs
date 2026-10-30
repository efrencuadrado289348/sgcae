using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;

namespace SGCAE.Application.Credentials;

public sealed record BlockCredentialCommand(Guid CredentialId) : ICommand<Unit>;
public sealed record UnblockCredentialCommand(Guid CredentialId) : ICommand<Unit>;
public sealed record RevokeCredentialCommand(Guid CredentialId) : ICommand<Unit>;

internal sealed class BlockCredentialHandler(ICredentialRepository credentials, IUnitOfWork uow, TimeProvider clock)
    : ICommandHandler<BlockCredentialCommand, Unit>
{
    public async Task<Unit> HandleAsync(BlockCredentialCommand cmd, CancellationToken ct)
    {
        var credential = await credentials.GetByIdAsync(cmd.CredentialId, ct)
            ?? throw new NotFoundException("Credencial", cmd.CredentialId);

        credential.Block(clock.GetUtcNow().UtcDateTime);
        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

internal sealed class UnblockCredentialHandler(ICredentialRepository credentials, IUnitOfWork uow, TimeProvider clock)
    : ICommandHandler<UnblockCredentialCommand, Unit>
{
    public async Task<Unit> HandleAsync(UnblockCredentialCommand cmd, CancellationToken ct)
    {
        var credential = await credentials.GetByIdAsync(cmd.CredentialId, ct)
            ?? throw new NotFoundException("Credencial", cmd.CredentialId);

        // Al reactivarla se vuelve a exigir la regla de "una sola credencial activa".
        if (await credentials.EmployeeHasActiveCredentialAsync(credential.EmployeeId, ct))
            throw new ConflictException("El empleado ya tiene otra credencial activa.");

        credential.Unblock(clock.GetUtcNow().UtcDateTime);
        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

internal sealed class RevokeCredentialHandler(ICredentialRepository credentials, IUnitOfWork uow, TimeProvider clock)
    : ICommandHandler<RevokeCredentialCommand, Unit>
{
    public async Task<Unit> HandleAsync(RevokeCredentialCommand cmd, CancellationToken ct)
    {
        var credential = await credentials.GetByIdAsync(cmd.CredentialId, ct)
            ?? throw new NotFoundException("Credencial", cmd.CredentialId);

        credential.Revoke(clock.GetUtcNow().UtcDateTime);
        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

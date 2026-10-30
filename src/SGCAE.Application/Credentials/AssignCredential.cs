using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;
using SGCAE.Application.Employees;
using SGCAE.Domain.Common;
using SGCAE.Domain.Credentials;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Credentials;

public sealed record AssignCredentialCommand(Guid EmployeeId, string Code) : ICommand<Guid>;

internal sealed class AssignCredentialHandler(
    ICredentialRepository credentials, IEmployeeRepository employees, IUnitOfWork uow, TimeProvider clock)
    : ICommandHandler<AssignCredentialCommand, Guid>
{
    public async Task<Guid> HandleAsync(AssignCredentialCommand cmd, CancellationToken ct)
    {
        var code = CredentialCode.Create(cmd.Code);

        var employee = await employees.GetByIdAsync(cmd.EmployeeId, ct)
            ?? throw new NotFoundException("Empleado", cmd.EmployeeId);
        if (!employee.IsActive)
            throw new DomainException("No se puede asignar una credencial a un empleado inactivo.");

        if (await credentials.ExistsByCodeAsync(code, ct))
            throw new ConflictException($"Ya existe una credencial con el código {code}.");
        if (await credentials.EmployeeHasActiveCredentialAsync(employee.Id, ct))
            throw new ConflictException("El empleado ya tiene una credencial activa. Bloquéela o revóquela primero.");

        var credential = Credential.Issue(code, employee.Id, clock.GetUtcNow().UtcDateTime);
        credentials.Add(credential);
        await uow.SaveChangesAsync(ct);
        return credential.Id;
    }
}

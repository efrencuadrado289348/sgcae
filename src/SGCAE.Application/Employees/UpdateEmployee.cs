using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Employees;

public sealed record UpdateEmployeeCommand(
    Guid Id, string FirstName, string LastName,
    string Email, string Position, string Area) : ICommand<Unit>;

internal sealed class UpdateEmployeeHandler(IEmployeeRepository employees, IUnitOfWork uow)
    : ICommandHandler<UpdateEmployeeCommand, Unit>
{
    public async Task<Unit> HandleAsync(UpdateEmployeeCommand cmd, CancellationToken ct)
    {
        var employee = await employees.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException("Empleado", cmd.Id);

        var email = Email.Create(cmd.Email);
        if (await employees.ExistsByEmailAsync(email, employee.Id, ct))
            throw new ConflictException($"El correo {email} ya está registrado.");

        employee.Update(cmd.FirstName, cmd.LastName, email, cmd.Position, cmd.Area);
        await uow.SaveChangesAsync(ct); // EF detecta los cambios del agregado rastreado
        return Unit.Value;
    }
}

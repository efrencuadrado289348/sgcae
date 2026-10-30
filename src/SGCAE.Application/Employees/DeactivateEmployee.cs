using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;

namespace SGCAE.Application.Employees;

public sealed record DeactivateEmployeeCommand(Guid Id) : ICommand<Unit>;

internal sealed class DeactivateEmployeeHandler(IEmployeeRepository employees, IUnitOfWork uow)
    : ICommandHandler<DeactivateEmployeeCommand, Unit>
{
    public async Task<Unit> HandleAsync(DeactivateEmployeeCommand cmd, CancellationToken ct)
    {
        var employee = await employees.GetByIdAsync(cmd.Id, ct)
            ?? throw new NotFoundException("Empleado", cmd.Id);

        employee.Deactivate();
        await uow.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

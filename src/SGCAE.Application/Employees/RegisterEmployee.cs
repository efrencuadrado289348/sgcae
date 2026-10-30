using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;
using SGCAE.Domain.Employees;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Employees;

public sealed record RegisterEmployeeCommand(
    string Identification, string FirstName, string LastName,
    string Email, string Position, string Area) : ICommand<Guid>;

internal sealed class RegisterEmployeeHandler(IEmployeeRepository employees, IUnitOfWork uow)
    : ICommandHandler<RegisterEmployeeCommand, Guid>
{
    public async Task<Guid> HandleAsync(RegisterEmployeeCommand cmd, CancellationToken ct)
    {
        // 1. Construir objetos de valor (validan formato).
        var identification = Identification.Create(cmd.Identification);
        var email = Email.Create(cmd.Email);

        // 2. Reglas que requieren consultar la BD (unicidad) -> en la capa de aplicación.
        if (await employees.ExistsByIdentificationAsync(identification, ct))
            throw new ConflictException($"Ya existe un empleado con identificación {identification}.");
        if (await employees.ExistsByEmailAsync(email, null, ct))
            throw new ConflictException($"El correo {email} ya está registrado.");

        // 3. El agregado valida sus propias invariantes.
        var employee = Employee.Create(identification, cmd.FirstName, cmd.LastName, email, cmd.Position, cmd.Area);

        // 4. Persistir.
        employees.Add(employee);
        await uow.SaveChangesAsync(ct);
        return employee.Id;
    }
}

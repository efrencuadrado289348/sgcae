using SGCAE.Domain.Common;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Domain.Employees;

/// <summary>
/// Agregado Empleado (RF-01). IMPLEMENTACIÓN DE REFERENCIA: úsala como patrón para los demás agregados.
/// - El estado solo cambia mediante métodos con intención de negocio (no setters públicos).
/// - Las invariantes se validan dentro del agregado, nunca en el controlador.
/// </summary>
public sealed class Employee : AggregateRoot
{
    public const int NameMaxLength = 100;
    public const int PositionMaxLength = 100;
    public const int AreaMaxLength = 100;

    public Identification Identification { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string Position { get; private set; } = null!;
    public string Area { get; private set; } = null!;
    public EmploymentStatus Status { get; private set; }

    public bool IsActive => Status == EmploymentStatus.Active;

    private Employee() { } // Requerido por EF Core

    public static Employee Create(
        Identification identification, string firstName, string lastName,
        Email email, string position, string area)
    {
        var employee = new Employee
        {
            Identification = identification,
            Status = EmploymentStatus.Active
        };
        employee.SetData(firstName, lastName, email, position, area);
        return employee;
    }

    public void Update(string firstName, string lastName, Email email, string position, string area)
    {
        if (!IsActive)
            throw new DomainException("No se puede actualizar un empleado inactivo.");

        SetData(firstName, lastName, email, position, area);
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("El empleado ya se encuentra inactivo.");

        Status = EmploymentStatus.Inactive;
    }

    private void SetData(string firstName, string lastName, Email email, string position, string area)
    {
        FirstName = Guard.Required(firstName, "nombres", NameMaxLength);
        LastName = Guard.Required(lastName, "apellidos", NameMaxLength);
        Email = email;
        Position = Guard.Required(position, "cargo", PositionMaxLength);
        Area = Guard.Required(area, "área", AreaMaxLength);
    }
}

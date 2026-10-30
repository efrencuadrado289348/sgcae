using SGCAE.Application.Common;
using SGCAE.Domain.Employees;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Employees;

public sealed record EmployeeDto(
    Guid Id, string Identification, string FirstName, string LastName,
    string Email, string Position, string Area, string Status);

/// <summary>Lado de ESCRITURA: trabaja con el agregado completo.</summary>
public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsByIdentificationAsync(Identification identification, CancellationToken ct);
    Task<bool> ExistsByEmailAsync(Email email, Guid? excludeId, CancellationToken ct);
    void Add(Employee employee);
}

/// <summary>Lado de LECTURA: devuelve DTOs directamente, sin cargar agregados ni hacer tracking.</summary>
public interface IEmployeeQueries
{
    Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PagedResult<EmployeeDto>> ListAsync(EmploymentStatus? status, string? area, int page, int pageSize, CancellationToken ct);
}

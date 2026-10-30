using Microsoft.EntityFrameworkCore;
using SGCAE.Application.Common;
using SGCAE.Application.Employees;
using SGCAE.Domain.Employees;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Queries;

/// <summary>Lado de LECTURA (REFERENCIA): AsNoTracking + paginación en BD + mapeo a DTO.</summary>
internal sealed class EmployeeQueries(SgcaeDbContext db) : IEmployeeQueries
{
    public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var employee = await db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        return employee is null ? null : ToDto(employee);
    }

    public async Task<PagedResult<EmployeeDto>> ListAsync(
        EmploymentStatus? status, string? area, int page, int pageSize, CancellationToken ct)
    {
        var query = db.Employees.AsNoTracking();

        if (status is not null) query = query.Where(e => e.Status == status);
        if (!string.IsNullOrWhiteSpace(area)) query = query.Where(e => e.Area == area.Trim());

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)   // paginar SIEMPRE con orden estable
            .Skip((page - 1) * pageSize).Take(pageSize)           // se traduce a OFFSET/FETCH en SQL
            .ToListAsync(ct);

        return new PagedResult<EmployeeDto>(items.Select(ToDto).ToList(), page, pageSize, total);
    }

    // El mapeo se hace en memoria porque EF no traduce ".Value" de un objeto de valor convertido.
    private static EmployeeDto ToDto(Employee e) => new(
        e.Id, e.Identification.Value, e.FirstName, e.LastName,
        e.Email.Value, e.Position, e.Area, e.Status.ToString());
}

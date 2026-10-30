using Microsoft.EntityFrameworkCore;
using SGCAE.Application.Access;
using SGCAE.Application.AccessPoints;
using SGCAE.Application.Credentials;
using SGCAE.Application.Employees;
using SGCAE.Domain.AccessPoints;
using SGCAE.Domain.AccessRecords;
using SGCAE.Domain.Credentials;
using SGCAE.Domain.Employees;
using SGCAE.Domain.ValueObjects;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Repositories;

// Repositorios = lado de ESCRITURA. Devuelven agregados con tracking para que EF detecte cambios.
// No llaman SaveChanges: eso lo decide el caso de uso a través de IUnitOfWork.

internal sealed class EmployeeRepository(SgcaeDbContext db) : IEmployeeRepository
{
    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<bool> ExistsByIdentificationAsync(Identification identification, CancellationToken ct) =>
        db.Employees.AnyAsync(e => e.Identification == identification, ct);

    public Task<bool> ExistsByEmailAsync(Email email, Guid? excludeId, CancellationToken ct) =>
        db.Employees.AnyAsync(e => e.Email == email && (excludeId == null || e.Id != excludeId), ct);

    public void Add(Employee employee) => db.Employees.Add(employee);
}

internal sealed class CredentialRepository(SgcaeDbContext db) : ICredentialRepository
{
    public Task<Credential?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Credentials.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Credential?> GetByCodeAsync(CredentialCode code, CancellationToken ct) =>
        db.Credentials.FirstOrDefaultAsync(c => c.Code == code, ct);

    public Task<bool> ExistsByCodeAsync(CredentialCode code, CancellationToken ct) =>
        db.Credentials.AnyAsync(c => c.Code == code, ct);

    public Task<bool> EmployeeHasActiveCredentialAsync(Guid employeeId, CancellationToken ct) =>
        db.Credentials.AnyAsync(c => c.EmployeeId == employeeId && c.Status == CredentialStatus.Active, ct);

    public void Add(Credential credential) => db.Credentials.Add(credential);
}

internal sealed class AccessPointRepository(SgcaeDbContext db) : IAccessPointRepository
{
    public Task<AccessPoint?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.AccessPoints.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<AccessPoint?> GetByCodeAsync(string code, CancellationToken ct) =>
        db.AccessPoints.FirstOrDefaultAsync(p => p.Code == code, ct);

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken ct) =>
        db.AccessPoints.AnyAsync(p => p.Code == code, ct);

    public void Add(AccessPoint accessPoint) => db.AccessPoints.Add(accessPoint);
}

internal sealed class AccessRecordRepository(SgcaeDbContext db) : IAccessRecordRepository
{
    public Task<MovementType?> GetLastAuthorizedMovementAsync(Guid employeeId, CancellationToken ct) =>
        db.AccessRecords
            .Where(r => r.EmployeeId == employeeId && r.Result == AccessResult.Authorized)
            .OrderByDescending(r => r.OccurredAtUtc)
            .Select(r => (MovementType?)r.MovementType)
            .FirstOrDefaultAsync(ct);

    public void Add(AccessRecord record) => db.AccessRecords.Add(record);
}

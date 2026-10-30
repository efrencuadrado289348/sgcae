using Microsoft.EntityFrameworkCore;
using SGCAE.Application.Credentials;
using SGCAE.Domain.Credentials;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Queries;

internal sealed class CredentialQueries(SgcaeDbContext db) : ICredentialQueries
{
    public async Task<CredentialDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var credential = await db.Credentials.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
        return credential is null ? null : ToDto(credential);
    }

    public async Task<IReadOnlyList<CredentialDto>> ListByEmployeeAsync(Guid employeeId, CancellationToken ct)
    {
        var items = await db.Credentials.AsNoTracking()
            .Where(c => c.EmployeeId == employeeId)
            .OrderByDescending(c => c.IssuedAtUtc)
            .ToListAsync(ct);

        return items.Select(ToDto).ToList();
    }

    private static CredentialDto ToDto(Credential c) =>
        new(c.Id, c.Code.Value, c.EmployeeId, c.Status.ToString(), c.IssuedAtUtc, c.StatusChangedAtUtc);
}

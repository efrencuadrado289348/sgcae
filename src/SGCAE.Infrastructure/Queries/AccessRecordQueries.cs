using Microsoft.EntityFrameworkCore;
using SGCAE.Application.Access;
using SGCAE.Application.Common;
using SGCAE.Domain.AccessRecords;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Queries;

/// <summary>RF-04: los filtros y la paginación se ejecutan en SQL Server, no en memoria.</summary>
internal sealed class AccessRecordQueries(SgcaeDbContext db) : IAccessRecordQueries
{
    public async Task<PagedResult<AccessRecordDto>> SearchAsync(
        AccessRecordFilter filter, int page, int pageSize, CancellationToken ct)
    {
        var query = db.AccessRecords.AsNoTracking();

        if (filter.EmployeeId is not null) query = query.Where(r => r.EmployeeId == filter.EmployeeId);
        if (filter.FromUtc is not null) query = query.Where(r => r.OccurredAtUtc >= filter.FromUtc);
        if (filter.ToUtc is not null) query = query.Where(r => r.OccurredAtUtc <= filter.ToUtc);
        if (filter.AccessPointId is not null) query = query.Where(r => r.AccessPointId == filter.AccessPointId);
        if (filter.Result is not null) query = query.Where(r => r.Result == filter.Result);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(r => r.OccurredAtUtc).ThenBy(r => r.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<AccessRecordDto>(items.Select(ToDto).ToList(), page, pageSize, total);
    }

    private static AccessRecordDto ToDto(AccessRecord r) => new(
        r.Id, r.CredentialCode, r.AccessPointCode, r.EmployeeId, r.AccessPointId,
        r.MovementType.ToString(), r.Result.ToString(), r.RejectionReason?.ToString(), r.OccurredAtUtc);
}

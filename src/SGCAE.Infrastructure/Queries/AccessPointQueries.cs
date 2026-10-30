using Microsoft.EntityFrameworkCore;
using SGCAE.Application.AccessPoints;
using SGCAE.Domain.AccessPoints;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Queries;

internal sealed class AccessPointQueries(SgcaeDbContext db) : IAccessPointQueries
{
    public async Task<AccessPointDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var point = await db.AccessPoints.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);
        return point is null ? null : ToDto(point);
    }

    public async Task<IReadOnlyList<AccessPointDto>> ListAsync(bool? isEnabled, CancellationToken ct)
    {
        var query = db.AccessPoints.AsNoTracking();
        if (isEnabled is not null) query = query.Where(p => p.IsEnabled == isEnabled);

        var items = await query.OrderBy(p => p.Code).ToListAsync(ct);
        return items.Select(ToDto).ToList();
    }

    private static AccessPointDto ToDto(AccessPoint p) => new(p.Id, p.Code, p.Name, p.Type.ToString(), p.IsEnabled);
}

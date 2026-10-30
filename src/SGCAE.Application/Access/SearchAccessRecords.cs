using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;
using SGCAE.Domain.AccessRecords;
using SGCAE.Domain.Common;

namespace SGCAE.Application.Access;

/// <summary>RF-04: historial filtrable y paginado.</summary>
public sealed record SearchAccessRecordsQuery(
    Guid? EmployeeId, DateTime? FromUtc, DateTime? ToUtc, Guid? AccessPointId, AccessResult? Result,
    int Page = 1, int PageSize = 20) : IQuery<PagedResult<AccessRecordDto>>;

internal sealed class SearchAccessRecordsHandler(IAccessRecordQueries queries)
    : IQueryHandler<SearchAccessRecordsQuery, PagedResult<AccessRecordDto>>
{
    public Task<PagedResult<AccessRecordDto>> HandleAsync(SearchAccessRecordsQuery q, CancellationToken ct)
    {
        if (q.FromUtc > q.ToUtc)
            throw new DomainException("La fecha inicial no puede ser mayor que la fecha final.");

        var (page, size) = Paging.Normalize(q.Page, q.PageSize);
        var filter = new AccessRecordFilter(q.EmployeeId, q.FromUtc, q.ToUtc, q.AccessPointId, q.Result);
        return queries.SearchAsync(filter, page, size, ct);
    }
}

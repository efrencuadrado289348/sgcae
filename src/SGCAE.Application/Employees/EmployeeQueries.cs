using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;
using SGCAE.Domain.Employees;

namespace SGCAE.Application.Employees;

public sealed record GetEmployeeByIdQuery(Guid Id) : IQuery<EmployeeDto>;

internal sealed class GetEmployeeByIdHandler(IEmployeeQueries queries)
    : IQueryHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    public async Task<EmployeeDto> HandleAsync(GetEmployeeByIdQuery query, CancellationToken ct) =>
        await queries.GetByIdAsync(query.Id, ct) ?? throw new NotFoundException("Empleado", query.Id);
}

public sealed record ListEmployeesQuery(EmploymentStatus? Status, string? Area, int Page = 1, int PageSize = 20)
    : IQuery<PagedResult<EmployeeDto>>;

internal sealed class ListEmployeesHandler(IEmployeeQueries queries)
    : IQueryHandler<ListEmployeesQuery, PagedResult<EmployeeDto>>
{
    public Task<PagedResult<EmployeeDto>> HandleAsync(ListEmployeesQuery query, CancellationToken ct)
    {
        var (page, size) = Paging.Normalize(query.Page, query.PageSize);
        return queries.ListAsync(query.Status, query.Area, page, size, ct);
    }
}

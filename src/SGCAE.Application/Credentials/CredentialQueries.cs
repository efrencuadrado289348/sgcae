using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;

namespace SGCAE.Application.Credentials;

public sealed record GetCredentialByIdQuery(Guid Id) : IQuery<CredentialDto>;
public sealed record ListEmployeeCredentialsQuery(Guid EmployeeId) : IQuery<IReadOnlyList<CredentialDto>>;

internal sealed class GetCredentialByIdHandler(ICredentialQueries queries)
    : IQueryHandler<GetCredentialByIdQuery, CredentialDto>
{
    public async Task<CredentialDto> HandleAsync(GetCredentialByIdQuery query, CancellationToken ct) =>
        await queries.GetByIdAsync(query.Id, ct) ?? throw new NotFoundException("Credencial", query.Id);
}

internal sealed class ListEmployeeCredentialsHandler(ICredentialQueries queries)
    : IQueryHandler<ListEmployeeCredentialsQuery, IReadOnlyList<CredentialDto>>
{
    public Task<IReadOnlyList<CredentialDto>> HandleAsync(ListEmployeeCredentialsQuery query, CancellationToken ct) =>
        queries.ListByEmployeeAsync(query.EmployeeId, ct);
}

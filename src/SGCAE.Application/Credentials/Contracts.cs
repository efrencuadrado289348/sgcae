using SGCAE.Domain.Credentials;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Credentials;

public sealed record CredentialDto(Guid Id, string Code, Guid EmployeeId, string Status,
    DateTime IssuedAtUtc, DateTime? StatusChangedAtUtc);

public interface ICredentialRepository
{
    Task<Credential?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<Credential?> GetByCodeAsync(CredentialCode code, CancellationToken ct);
    Task<bool> ExistsByCodeAsync(CredentialCode code, CancellationToken ct);
    Task<bool> EmployeeHasActiveCredentialAsync(Guid employeeId, CancellationToken ct);
    void Add(Credential credential);
}

public interface ICredentialQueries
{
    Task<CredentialDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<CredentialDto>> ListByEmployeeAsync(Guid employeeId, CancellationToken ct);
}

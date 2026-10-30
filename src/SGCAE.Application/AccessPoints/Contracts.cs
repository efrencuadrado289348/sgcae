using SGCAE.Domain.AccessPoints;

namespace SGCAE.Application.AccessPoints;

public sealed record AccessPointDto(Guid Id, string Code, string Name, string Type, bool IsEnabled);

public interface IAccessPointRepository
{
    Task<AccessPoint?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<AccessPoint?> GetByCodeAsync(string code, CancellationToken ct);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct);
    void Add(AccessPoint accessPoint);
}

public interface IAccessPointQueries
{
    Task<AccessPointDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<AccessPointDto>> ListAsync(bool? isEnabled, CancellationToken ct);
}

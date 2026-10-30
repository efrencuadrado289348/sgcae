using SGCAE.Application.Common;
using SGCAE.Domain.AccessRecords;

namespace SGCAE.Application.Access;

public sealed record AccessDecisionDto(Guid RecordId, string Result, string? RejectionReason, DateTime OccurredAtUtc);

public sealed record AccessRecordDto(
    Guid Id, string CredentialCode, string AccessPointCode, Guid? EmployeeId, Guid? AccessPointId,
    string MovementType, string Result, string? RejectionReason, DateTime OccurredAtUtc);

public sealed record AccessRecordFilter(
    Guid? EmployeeId, DateTime? FromUtc, DateTime? ToUtc, Guid? AccessPointId, AccessResult? Result);

public interface IAccessRecordRepository
{
    /// <summary>Tipo del último movimiento AUTORIZADO del empleado (para validar la secuencia).</summary>
    Task<MovementType?> GetLastAuthorizedMovementAsync(Guid employeeId, CancellationToken ct);
    void Add(AccessRecord record);
}

public interface IAccessRecordQueries
{
    Task<PagedResult<AccessRecordDto>> SearchAsync(AccessRecordFilter filter, int page, int pageSize, CancellationToken ct);
}

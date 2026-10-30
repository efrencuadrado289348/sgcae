using SGCAE.Domain.Common;

namespace SGCAE.Domain.AccessRecords;

/// <summary>
/// Registro de un intento de acceso (RF-03). Es INMUTABLE: es un hecho histórico de auditoría.
/// Se guardan los códigos tal como llegaron, porque en un rechazo la credencial o el punto
/// pueden no existir (por eso EmployeeId y AccessPointId son opcionales).
/// </summary>
public sealed class AccessRecord : AggregateRoot
{
    public string CredentialCode { get; private set; } = null!;
    public string AccessPointCode { get; private set; } = null!;
    public Guid? EmployeeId { get; private set; }
    public Guid? AccessPointId { get; private set; }
    public MovementType MovementType { get; private set; }
    public AccessResult Result { get; private set; }
    public RejectionReason? RejectionReason { get; private set; }
    public DateTime OccurredAtUtc { get; private set; }

    private AccessRecord() { }

    public static AccessRecord Authorized(
        string credentialCode, string accessPointCode, Guid employeeId, Guid accessPointId,
        MovementType movement, DateTime nowUtc) =>
        new()
        {
            CredentialCode = credentialCode,
            AccessPointCode = accessPointCode,
            EmployeeId = employeeId,
            AccessPointId = accessPointId,
            MovementType = movement,
            Result = AccessResult.Authorized,
            OccurredAtUtc = nowUtc
        };

    public static AccessRecord Rejected(
        string credentialCode, string accessPointCode, Guid? employeeId, Guid? accessPointId,
        MovementType movement, RejectionReason reason, DateTime nowUtc) =>
        new()
        {
            CredentialCode = credentialCode,
            AccessPointCode = accessPointCode,
            EmployeeId = employeeId,
            AccessPointId = accessPointId,
            MovementType = movement,
            Result = AccessResult.Rejected,
            RejectionReason = reason,
            OccurredAtUtc = nowUtc
        };
}

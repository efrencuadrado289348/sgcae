namespace SGCAE.Domain.AccessRecords;

public enum AccessResult
{
    Authorized = 1,
    Rejected = 2
}

public enum RejectionReason
{
    CredentialNotFound = 1,
    CredentialBlocked = 2,
    CredentialRevoked = 3,
    EmployeeInactive = 4,
    AccessPointNotFound = 5,
    AccessPointDisabled = 6,
    /// <summary>Entrada sin salida previa, o salida sin entrada (evita registros duplicados).</summary>
    InvalidMovementSequence = 7
}

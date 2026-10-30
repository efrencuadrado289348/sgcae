using SGCAE.Domain.AccessPoints;
using SGCAE.Domain.AccessRecords;
using SGCAE.Domain.Credentials;
using SGCAE.Domain.Employees;

namespace SGCAE.Domain.Services;

/// <summary>
/// Servicio de dominio (RF-03): la regla "¿puede pasar?" involucra VARIOS agregados
/// (credencial, empleado, punto, último movimiento), por eso no pertenece a ninguno de ellos.
/// Es una función pura: sin base de datos, fácil de probar.
/// </summary>
public static class AccessPolicy
{
    /// <returns>null si el acceso se autoriza; el motivo de rechazo en caso contrario.</returns>
    public static RejectionReason? Evaluate(
        Credential? credential,
        Employee? employee,
        AccessPoint? accessPoint,
        MovementType requested,
        MovementType? lastAuthorizedMovement)
    {
        if (credential is null) return RejectionReason.CredentialNotFound;
        if (credential.Status == CredentialStatus.Revoked) return RejectionReason.CredentialRevoked;
        if (credential.Status == CredentialStatus.Blocked) return RejectionReason.CredentialBlocked;
        if (employee is null || !employee.IsActive) return RejectionReason.EmployeeInactive;
        if (accessPoint is null) return RejectionReason.AccessPointNotFound;
        if (!accessPoint.IsEnabled) return RejectionReason.AccessPointDisabled;

        // Secuencia válida: Entrada si está afuera (sin historial o último fue Salida); Salida si está adentro.
        var isInside = lastAuthorizedMovement == MovementType.Entry;
        var validSequence = requested == MovementType.Entry ? !isInside : isInside;
        return validSequence ? null : RejectionReason.InvalidMovementSequence;
    }
}

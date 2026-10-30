using SGCAE.Domain.Common;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Domain.Credentials;

/// <summary>
/// Agregado Credencial (RF-02). Referencia al empleado solo por Id (agregados distintos).
/// Máquina de estados:  Active ⇄ Blocked,  Active/Blocked → Revoked (final).
/// </summary>
public sealed class Credential : AggregateRoot
{
    public CredentialCode Code { get; private set; } = null!;
    public Guid EmployeeId { get; private set; }
    public CredentialStatus Status { get; private set; }
    public DateTime IssuedAtUtc { get; private set; }
    public DateTime? StatusChangedAtUtc { get; private set; }

    /// <summary>Solo una credencial activa puede usarse para acceder.</summary>
    public bool CanBeUsed => Status == CredentialStatus.Active;

    private Credential() { }

    public static Credential Issue(CredentialCode code, Guid employeeId, DateTime nowUtc)
    {
        if (employeeId == Guid.Empty)
            throw new DomainException("La credencial debe pertenecer a un empleado.");

        return new Credential
        {
            Code = code,
            EmployeeId = employeeId,
            Status = CredentialStatus.Active,
            IssuedAtUtc = nowUtc
        };
    }

    public void Block(DateTime nowUtc)
    {
        if (Status == CredentialStatus.Revoked)
            throw new DomainException("Una credencial revocada no puede bloquearse.");
        if (Status == CredentialStatus.Blocked)
            throw new DomainException("La credencial ya se encuentra bloqueada.");

        ChangeStatus(CredentialStatus.Blocked, nowUtc);
    }

    public void Unblock(DateTime nowUtc)
    {
        if (Status != CredentialStatus.Blocked)
            throw new DomainException("Solo una credencial bloqueada puede desbloquearse.");

        ChangeStatus(CredentialStatus.Active, nowUtc);
    }

    public void Revoke(DateTime nowUtc)
    {
        if (Status == CredentialStatus.Revoked)
            throw new DomainException("La credencial ya está revocada.");

        ChangeStatus(CredentialStatus.Revoked, nowUtc);
    }

    private void ChangeStatus(CredentialStatus status, DateTime nowUtc)
    {
        Status = status;
        StatusChangedAtUtc = nowUtc;
    }
}

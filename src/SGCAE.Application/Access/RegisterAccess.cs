using SGCAE.Application.Abstractions;
using SGCAE.Application.AccessPoints;
using SGCAE.Application.Credentials;
using SGCAE.Application.Employees;
using SGCAE.Domain.AccessRecords;
using SGCAE.Domain.Common;
using SGCAE.Domain.Credentials;
using SGCAE.Domain.Services;
using SGCAE.Domain.ValueObjects;

namespace SGCAE.Application.Access;

/// <summary>RF-03: lo que envía el lector físico en cada intento de entrada/salida.</summary>
public sealed record RegisterAccessCommand(string CredentialCode, string AccessPointCode, MovementType MovementType)
    : ICommand<AccessDecisionDto>;

/// <summary>
/// Un rechazo NO es una excepción: es un resultado de negocio que se guarda en el historial.
/// Solo se lanza excepción si la petición en sí es inválida (tipo de movimiento inexistente).
/// </summary>
internal sealed class RegisterAccessHandler(
    ICredentialRepository credentials,
    IEmployeeRepository employees,
    IAccessPointRepository points,
    IAccessRecordRepository records,
    IUnitOfWork uow,
    TimeProvider clock) : ICommandHandler<RegisterAccessCommand, AccessDecisionDto>
{
    private const int StoredCodeMaxLength = 50; // longitud de las columnas de auditoría

    public async Task<AccessDecisionDto> HandleAsync(RegisterAccessCommand cmd, CancellationToken ct)
    {
        if (!Enum.IsDefined(cmd.MovementType))
            throw new DomainException("El tipo de movimiento debe ser Entry o Exit.");

        var credentialCode = Normalize(cmd.CredentialCode);
        var accessPointCode = Normalize(cmd.AccessPointCode);

        // 1. Cargar todo lo necesario para decidir (sin lanzar excepciones si algo no existe).
        var credential = await FindCredentialAsync(credentialCode, ct);
        var employee = credential is null ? null : await employees.GetByIdAsync(credential.EmployeeId, ct);
        var accessPoint = await points.GetByCodeAsync(accessPointCode, ct);
        var lastMovement = employee is null ? null : await records.GetLastAuthorizedMovementAsync(employee.Id, ct);

        // 2. La decisión es del dominio.
        var reason = AccessPolicy.Evaluate(credential, employee, accessPoint, cmd.MovementType, lastMovement);

        // 3. Registrar SIEMPRE el intento (autorizado o rechazado).
        var now = clock.GetUtcNow().UtcDateTime;
        var record = reason is null
            ? AccessRecord.Authorized(credentialCode, accessPointCode, employee!.Id, accessPoint!.Id, cmd.MovementType, now)
            : AccessRecord.Rejected(credentialCode, accessPointCode, employee?.Id, accessPoint?.Id, cmd.MovementType, reason.Value, now);

        records.Add(record);
        await uow.SaveChangesAsync(ct);

        return new AccessDecisionDto(record.Id, record.Result.ToString(), record.RejectionReason?.ToString(), record.OccurredAtUtc);
    }

    private async Task<Credential?> FindCredentialAsync(string code, CancellationToken ct)
    {
        try
        {
            return await credentials.GetByCodeAsync(CredentialCode.Create(code), ct);
        }
        catch (DomainException)
        {
            return null; // formato inválido = credencial inexistente; el intento igual se registra
        }
    }

    private static string Normalize(string? value)
    {
        var normalized = (value ?? string.Empty).Trim().ToUpperInvariant();
        return normalized.Length > StoredCodeMaxLength ? normalized[..StoredCodeMaxLength] : normalized;
    }
}

using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Access;
using SGCAE.Application.Common;
using SGCAE.Domain.AccessRecords;

namespace SGCAE.Api.Controllers;

/// <summary>RF-03 y RF-04.</summary>
[ApiController]
[Route("api/access")]
public sealed class AccessController(IDispatcher dispatcher) : ControllerBase
{
    /// <summary>
    /// Solicitud desde un lector. Responde 200 tanto si se autoriza como si se rechaza:
    /// un rechazo es un resultado de negocio válido (y queda registrado), no un error HTTP.
    /// </summary>
    [HttpPost("requests")]
    public Task<AccessDecisionDto> RequestAccess(RegisterAccessCommand command, CancellationToken ct) =>
        dispatcher.SendAsync(command, ct);

    [HttpGet("records")]
    public Task<PagedResult<AccessRecordDto>> Search(
        [FromQuery] Guid? employeeId, [FromQuery] DateTime? fromUtc, [FromQuery] DateTime? toUtc,
        [FromQuery] Guid? accessPointId, [FromQuery] AccessResult? result,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        dispatcher.QueryAsync(new SearchAccessRecordsQuery(employeeId, fromUtc, toUtc, accessPointId, result, page, pageSize), ct);
}

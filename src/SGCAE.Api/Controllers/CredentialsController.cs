using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Credentials;

namespace SGCAE.Api.Controllers;

/// <summary>RF-02.</summary>
[ApiController]
public sealed class CredentialsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost("api/employees/{employeeId:guid}/credentials")]
    public async Task<IActionResult> Assign(Guid employeeId, AssignCredentialRequest request, CancellationToken ct)
    {
        var id = await dispatcher.SendAsync(new AssignCredentialCommand(employeeId, request.Code), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("api/employees/{employeeId:guid}/credentials")]
    public Task<IReadOnlyList<CredentialDto>> ListByEmployee(Guid employeeId, CancellationToken ct) =>
        dispatcher.QueryAsync(new ListEmployeeCredentialsQuery(employeeId), ct);

    [HttpGet("api/credentials/{id:guid}")]
    public Task<CredentialDto> GetById(Guid id, CancellationToken ct) =>
        dispatcher.QueryAsync(new GetCredentialByIdQuery(id), ct);

    [HttpPost("api/credentials/{id:guid}/block")]
    public async Task<IActionResult> Block(Guid id, CancellationToken ct)
    {
        await dispatcher.SendAsync(new BlockCredentialCommand(id), ct);
        return NoContent();
    }

    [HttpPost("api/credentials/{id:guid}/unblock")]
    public async Task<IActionResult> Unblock(Guid id, CancellationToken ct)
    {
        await dispatcher.SendAsync(new UnblockCredentialCommand(id), ct);
        return NoContent();
    }

    [HttpPost("api/credentials/{id:guid}/revoke")]
    public async Task<IActionResult> Revoke(Guid id, CancellationToken ct)
    {
        await dispatcher.SendAsync(new RevokeCredentialCommand(id), ct);
        return NoContent();
    }
}

public sealed record AssignCredentialRequest(string Code);

using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.Abstractions;
using SGCAE.Application.AccessPoints;

namespace SGCAE.Api.Controllers;

/// <summary>RF-06.</summary>
[ApiController]
[Route("api/access-points")]
public sealed class AccessPointsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register(RegisterAccessPointCommand command, CancellationToken ct)
    {
        var id = await dispatcher.SendAsync(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public Task<AccessPointDto> GetById(Guid id, CancellationToken ct) =>
        dispatcher.QueryAsync(new GetAccessPointByIdQuery(id), ct);

    /// <param name="isEnabled">true = solo habilitados, false = solo deshabilitados, vacío = todos.</param>
    [HttpGet]
    public Task<IReadOnlyList<AccessPointDto>> List([FromQuery] bool? isEnabled, CancellationToken ct) =>
        dispatcher.QueryAsync(new ListAccessPointsQuery(isEnabled), ct);

    [HttpPatch("{id:guid}/enable")]
    public async Task<IActionResult> Enable(Guid id, CancellationToken ct)
    {
        await dispatcher.SendAsync(new SetAccessPointEnabledCommand(id, true), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/disable")]
    public async Task<IActionResult> Disable(Guid id, CancellationToken ct)
    {
        await dispatcher.SendAsync(new SetAccessPointEnabledCommand(id, false), ct);
        return NoContent();
    }
}

using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;
using SGCAE.Application.Employees;
using SGCAE.Domain.Employees;

namespace SGCAE.Api.Controllers;

/// <summary>RF-01. Controlador delgado: solo traduce HTTP ⇄ comando/consulta. Sin lógica de negocio.</summary>
[ApiController]
[Route("api/employees")]
public sealed class EmployeesController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(RegisterEmployeeCommand command, CancellationToken ct)
    {
        var id = await dispatcher.SendAsync(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpGet("{id:guid}")]
    public Task<EmployeeDto> GetById(Guid id, CancellationToken ct) =>
        dispatcher.QueryAsync(new GetEmployeeByIdQuery(id), ct);

    [HttpGet]
    public Task<PagedResult<EmployeeDto>> List(
        [FromQuery] EmploymentStatus? status, [FromQuery] string? area,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default) =>
        dispatcher.QueryAsync(new ListEmployeesQuery(status, area, page, pageSize), ct);

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateEmployeeRequest request, CancellationToken ct)
    {
        await dispatcher.SendAsync(new UpdateEmployeeCommand(
            id, request.FirstName, request.LastName, request.Email, request.Position, request.Area), ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await dispatcher.SendAsync(new DeactivateEmployeeCommand(id), ct);
        return NoContent();
    }
}

public sealed record UpdateEmployeeRequest(string FirstName, string LastName, string Email, string Position, string Area);

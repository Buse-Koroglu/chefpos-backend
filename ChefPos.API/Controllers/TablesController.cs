using ChefPos.Application.Tables.Commands.ActivateTable;
using ChefPos.Application.Tables.Commands.CreateTable;
using ChefPos.Application.Tables.Commands.DeactivateTable;
using ChefPos.Application.Tables.Commands.UpdateTable;
using ChefPos.Application.Tables.DTOs;
using ChefPos.Application.Tables.Queries.ExportTables;
using ChefPos.Application.Tables.Queries.GetTablesByLocation;
using ChefPos.Application.Tables.Queries.GetTablesPaged;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/tables")]
[Authorize]
public class TablesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TablesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<ActionResult> CreateTable(CreateTableCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,WAITER")]
    [HttpGet]
    public async Task<ActionResult> GetTablesByLocation([FromQuery] Guid locationId, [FromQuery] bool includeInactive,CancellationToken cancellationToken) {
        var result = await _mediator.Send(new GetTablesByLocationQuery(locationId, includeInactive), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("paged")]
    public async Task<ActionResult> GetTablesPaged([FromQuery] string? searchTerm, [FromQuery] Guid? locationId, [FromQuery] bool? isActive, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetTablesPagedQuery(searchTerm, locationId, isActive, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] string? searchTerm, [FromQuery] Guid? locationId, [FromQuery] bool? isActive, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ExportTablesQuery(searchTerm, locationId, isActive), cancellationToken);
        return File(result.Content, ChefPos.Application.Common.Export.ExportFileResult.ContentType, result.FileName);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTable([FromRoute] Guid id, UpdateTableRequestDto body, CancellationToken cancellationToken)
    {
        var command = new UpdateTableCommand(id, body.TableNumber);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("{id}/activate")]
    public async Task<ActionResult> ActivateTable([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ActivateTableCommand(id), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult> DeactivateTable([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeactivateTableCommand(id), cancellationToken);
        return Ok(result);
    }
}

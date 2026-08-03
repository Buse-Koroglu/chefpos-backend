using ChefPos.Application.StockRequests.Commands.ApproveStockRequest;
using ChefPos.Application.StockRequests.Commands.CreateStockRequest;
using ChefPos.Application.StockRequests.Commands.RejectStockRequest;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Application.StockRequests.Queries.GetStockRequestById;
using ChefPos.Application.StockRequests.Queries.GetStockRequests;
using ChefPos.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/stock-requests")]
public class StockRequestsController : ControllerBase
{
    private readonly IMediator _mediator;
    public StockRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> CreateStockRequest(CreateStockRequestCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetStockRequestById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetStockRequestByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetStockRequests([FromQuery] Guid locationId, [FromQuery] StockRequestStatus? status,CancellationToken cancellationToken)
    {
        var query = new GetStockRequestsQuery(locationId, status);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }


    [HttpPost("{id}/approve")]
    public async Task<ActionResult> ApproveStockRequest([FromRoute] Guid id, ApproveStockRequestDto body, CancellationToken cancellationToken)
    {
        var command = new ApproveStockRequestCommand(id, body.DecidedByUserId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/reject")]
    public async Task<ActionResult> RejectStockRequest([FromRoute] Guid id, RejectStockRequestDto body, CancellationToken cancellationToken)
    {
        var command = new RejectStockRequestCommand(id, body.DecidedByUserId, body.Reason);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
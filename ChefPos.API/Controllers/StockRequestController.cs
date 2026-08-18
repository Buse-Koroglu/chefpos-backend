using ChefPos.Application.Common.Pagination;
using ChefPos.Application.StockRequests.Commands.ApproveStockRequest;
using ChefPos.Application.StockRequests.Commands.CreateStockRequest;
using ChefPos.Application.StockRequests.Commands.RejectStockRequest;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Application.StockRequests.Queries.GetStockRequestById;
using ChefPos.Application.StockRequests.Queries.GetStockRequestPaged;
using ChefPos.Application.StockRequests.Queries.GetStockRequests;
using ChefPos.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/stock-requests")]
[Authorize]
public class StockRequestsController : ControllerBase
{
    private readonly IMediator _mediator;
    public StockRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "INVENTORY_STAFF")]
    [HttpPost]
    public async Task<ActionResult> CreateStockRequest(CreateStockRequestCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    
    [Authorize(Roles = "ADMIN,STOCK_MANAGER,INVENTORY_STAFF")]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetStockRequestById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetStockRequestByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,STOCK_MANAGER,INVENTORY_STAFF")]
    [HttpGet]
    public async Task<ActionResult> GetStockRequests([FromQuery] Guid locationId, [FromQuery] StockRequestStatus? status,CancellationToken cancellationToken)
    {
        var query = new GetStockRequestsQuery(locationId, status);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN,STOCK_MANAGER,INVENTORY_STAFF")]
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<AdminStockRequestResponseDto>>> GetAllPaged(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? locationId,
        [FromQuery] StockRequestStatus? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPagedStockRequestsQuery(searchTerm, locationId, status, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "STOCK_MANAGER")]
    [HttpPost("{id}/approve")]
    public async Task<ActionResult> ApproveStockRequest([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new ApproveStockRequestCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "STOCK_MANAGER")]
    [HttpPost("{id}/reject")]
    public async Task<ActionResult> RejectStockRequest([FromRoute] Guid id, RejectStockRequestDto body, CancellationToken cancellationToken)
    {
        var command = new RejectStockRequestCommand(id, body.Reason);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
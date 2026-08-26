
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.StockRequests.Commands.ApproveStockRequest;
using ChefPos.Application.StockRequests.Commands.CreateStockRequest;
using ChefPos.Application.StockRequests.Commands.RejectStockRequest;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Application.StockRequests.Queries.ExportStockRequests;
using ChefPos.Application.StockRequests.Queries.GetInventoryDashboardStats;
using ChefPos.Application.StockRequests.Queries.GetStockManagerDashboardStats;
using ChefPos.Application.StockRequests.Queries.GetStockRequestById;
using ChefPos.Application.StockRequests.Queries.GetStockRequestPaged;
using ChefPos.Application.StockRequests.Queries.GetStockRequests;
using ChefPos.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult<PagedResult<AdminStockRequestResponseDto>>> GetAllPaged([FromQuery] string? searchTerm,[FromQuery] Guid? locationId, [FromQuery] StockRequestStatus? status, [FromQuery] bool onlyMyRequests = false, [FromQuery] bool onlyHistory = false, [FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetPagedStockRequestsQuery(searchTerm, locationId, status, onlyMyRequests, onlyHistory, startDate, endDate, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? locationId,
        [FromQuery] StockRequestStatus? status,
        [FromQuery] bool onlyHistory = false,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = new ExportStockRequestsQuery(searchTerm, locationId, status, onlyHistory, startDate, endDate);
        var result = await _mediator.Send(query, cancellationToken);
        return File(result.Content, ChefPos.Application.Common.Export.ExportFileResult.ContentType, result.FileName);
    }

    [Authorize(Roles = "INVENTORY_STAFF,ADMIN")]
    [HttpGet("dashboard-stats")]
    public async Task<ActionResult<InventoryDashboardStatsDto>>
        GetInventoryDashboardStats(
            [FromQuery] Guid? locationId,
            CancellationToken cancellationToken)
    {
        var query = new GetInventoryDashboardStatsQuery(
            locationId);

        var result = await _mediator.Send(
            query,
            cancellationToken);

        return Ok(result);
    }

    [Authorize(Roles = "STOCK_MANAGER,ADMIN")]
    [HttpGet("stock-manager-dashboard-stats")]
    public async Task<ActionResult<StockManagerDashboardStatsDto>>
        GetStockManagerDashboardStats(
            [FromQuery] Guid locationId,
            CancellationToken cancellationToken)
    {
        var query = new GetStockManagerDashboardStatsQuery(locationId);

        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [Authorize(Roles = "STOCK_MANAGER")]
    [HttpPost("{id}/approve")]
    public async Task<ActionResult> ApproveStockRequest([FromRoute] Guid id, ApproveStockRequestDto body, CancellationToken cancellationToken)
    {
        var command = new ApproveStockRequestCommand(id, body.UnitPrice);
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
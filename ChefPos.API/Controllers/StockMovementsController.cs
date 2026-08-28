using ChefPos.Application.Common.Pagination;
using ChefPos.Application.StockMovements.DTOs;
using ChefPos.Application.StockMovements.Queries.GetStockMovementsPaged;
using ChefPos.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/stock-movements")]
[Authorize]
public class StockMovementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public StockMovementsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "ADMIN,STOCK_MANAGER,INVENTORY_STAFF")]
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<StockMovementResponseDto>>> GetPaged([FromQuery] Guid? ingredientId, [FromQuery] Guid? locationId, [FromQuery] StockMovementType? type, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = new GetStockMovementsPagedQuery(ingredientId, locationId, type, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}

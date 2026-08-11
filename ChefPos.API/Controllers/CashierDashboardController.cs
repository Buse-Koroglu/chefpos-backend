using ChefPos.Application.Orders.Queries.CashierDashboard;
using ChefPos.Application.Orders.Queries.GetWeeklyReveune;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class CashierDashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    public CashierDashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }
 
    [Authorize(Roles = "CASHIER,ADMIN")]
    [HttpGet("cashier")]
    public async Task<ActionResult> GetCashierDashboard([FromQuery] Guid locationId, CancellationToken cancellationToken)
    {
        var query = new GetCashierDashboardQuery(locationId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "CASHIER,ADMIN")]
    [HttpGet("weekly-revenue")]
    public async Task<ActionResult> GetWeeklyRevenue([FromQuery] Guid locationId, CancellationToken cancellationToken)
    {
        var query = new GetWeeklyRevenueQuery(locationId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
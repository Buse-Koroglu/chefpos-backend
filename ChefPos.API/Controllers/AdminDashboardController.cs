using ChefPos.Application.Dashboard.Admin.Queries.GetDashboardSummary;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class AdminDashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminDashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("summary")]
    public async Task<ActionResult> GetSummary([FromQuery] Guid locationId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetDashboardSummaryQuery(locationId), cancellationToken);
        return Ok(result);
    }
}
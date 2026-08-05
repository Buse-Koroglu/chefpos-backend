using ChefPos.Application.Locations.Commands.ActivateLocation;
using ChefPos.Application.Locations.Commands.CreateLocation;
using ChefPos.Application.Locations.Commands.DeactivateLocation;
using ChefPos.Application.Locations.Commands.UpdateLocation;
using ChefPos.Application.Locations.DTOs;
using ChefPos.Application.Locations.Queries.GetLocations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/locations")]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly IMediator _mediator;
    public LocationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<ActionResult> CreateLocation(CreateLocationCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetLocationById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetLocationByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult> GetLocations([FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = new GetLocationsQuery(includeInactive);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    

    [Authorize(Roles = "ADMIN")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateLocation([FromRoute] Guid id, UpdateLocationRequestDto body, CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand(id, body.Name);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("{id}/activate")]
    public async Task<ActionResult> ActivateLocation([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateLocationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult> DeactivateLocation([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateLocationCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
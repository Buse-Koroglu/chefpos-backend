using ChefPos.Application.Ingredients.Commands;
using ChefPos.Application.Ingredients.Commands.ActivateIngredient;
using ChefPos.Application.Ingredients.Commands.DeactivateIngredient;
using ChefPos.Application.Ingredients.Commands.UpdateIngredient;
using ChefPos.Application.Ingredients.Commands.UpdateIngredientMinStockThreshold;
using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Application.Ingredients.Queries.GetIngredientById;
using ChefPos.Application.Ingredients.Queries.GetIngredients;
using ChefPos.Application.Ingredients.Queries.GetIngredientsPaged;
using ChefPos.Application.Ingredients.Queries.GetLowStockIngredients;
using ChefPos.Application.Common.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/ingredients")]
[Authorize]
public class IngredientsController : ControllerBase
{
    private readonly IMediator _mediator;
    public IngredientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<ActionResult> CreateIngredient(CreateIngredientCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IngredientResponseDto>> GetIngredientById([FromRoute]Guid id, CancellationToken cancellationToken)
    {
        var query = new GetIngredientByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<List<IngredientResponseDto>>> GetAllIngredients([FromQuery]Guid locationId, [FromQuery] bool includeInactive = false, CancellationToken cancellationToken= default)
    {
        var query = new GetIngredientsQuery(locationId, includeInactive);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<IngredientAdminResponseDto>>> GetIngredientsPaged(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? locationId,
        [FromQuery] bool? isActive,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetIngredientsPagedQuery(searchTerm, locationId, isActive, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,STOCK_MANAGER,INVENTORY_STAFF")]
    [HttpGet("low-stock")]
    public async Task<ActionResult> GetLowStockIngredients([FromQuery] Guid locationId, CancellationToken cancellationToken)
    {
        var query = new GetLowStockIngredientsQuery(locationId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateIngredient([FromRoute] Guid id, UpdateIngredientRequest body, CancellationToken cancellationToken)
    {
        var command = new UpdateIngredientCommand(id, body.Name, body.UnitPrice);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    

    [Authorize(Roles = "ADMIN,STOCK_MANAGER")]
    [HttpPatch("{id}/min-stock-threshold")]
    public async Task<ActionResult> UpdateMinStockThreshold([FromRoute] Guid id, UpdateMinStockThresholdRequest body, CancellationToken cancellationToken)
    {
        var command = new UpdateIngredientMinStockThresholdCommand(id, body.MinStockThreshold);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("{id}/activate")]
    public async Task<ActionResult> ActivateIngredient([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateIngredientCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult> DeactivateIngredient([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateIngredientCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
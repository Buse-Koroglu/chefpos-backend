using ChefPos.Application.Ingredients.Commands;
using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Application.Ingredients.Queries.GetIngredientById;
using ChefPos.Application.Ingredients.Queries.GetIngredients;
using ChefPos.Application.Ingredients.Queries.GetLowStockIngredients;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/ingredients")]
public class IngredientsController : ControllerBase
{
    private readonly IMediator _mediator;
    public IngredientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

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
    
    [HttpGet("low-stock")]
    public async Task<ActionResult> GetLowStockIngredients([FromQuery] Guid locationId, CancellationToken cancellationToken)
    {
        var query = new GetLowStockIngredientsQuery(locationId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
using ChefPos.Application.Ingredients.Commands;
using ChefPos.Application.Ingredients.Commands.ActivateIngredient;
using ChefPos.Application.Ingredients.Commands.DeactivateIngredient;
using ChefPos.Application.Ingredients.Commands.UpdateIngredient;
using ChefPos.Application.Ingredients.Commands.UpdateIngredientMinStockThreshold;
using ChefPos.Application.Ingredients.Commands.UpdateIngredientPrice;
using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Application.Ingredients.Queries.ExportIngredients;
using ChefPos.Application.Ingredients.Queries.GetIngredientById;
using ChefPos.Application.Ingredients.Queries.GetIngredients;
using ChefPos.Application.Ingredients.Queries.GetIngredientsPaged;
using ChefPos.Application.Ingredients.Queries.GetLowStockIngredients;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Ingredients.Commands.RecordIngredientPurchase;
using ChefPos.Application.Ingredients.Commands.RecordManuelIngredientDeduction;
using ChefPos.Application.Ingredients.Commands.RecordProductProduction;
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
 
    [Authorize(Roles = "SUPER_ADMIN")]
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
 
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
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

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpGet("export")]
    public async Task<IActionResult> Export(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? locationId,
        [FromQuery] bool? isActive,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ExportIngredientsQuery(searchTerm, locationId, isActive), cancellationToken);
        return File(result.Content, ChefPos.Application.Common.Export.ExportFileResult.ContentType, result.FileName);
    }

    [Authorize(Roles = "ADMIN,STOCK_MANAGER,INVENTORY_STAFF")]
    [HttpGet("low-stock")]
    public async Task<ActionResult> GetLowStockIngredients([FromQuery] Guid locationId, CancellationToken cancellationToken)
    {
        var query = new GetLowStockIngredientsQuery(locationId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateIngredient([FromRoute] Guid id, UpdateIngredientRequest body, CancellationToken cancellationToken)
    {
        var command = new UpdateIngredientCommand(id, body.Name);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
 
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPatch("{id}/price")]
    public async Task<ActionResult> UpdatePrice([FromRoute] Guid id, UpdateIngredientPriceRequest body, CancellationToken cancellationToken)
    {
        var command = new UpdateIngredientPriceCommand(id, body.UnitPrice);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN,STOCK_MANAGER")]
    [HttpPatch("{id}/min-stock-threshold")]
    public async Task<ActionResult> UpdateMinStockThreshold([FromRoute] Guid id, UpdateMinStockThresholdRequest body, CancellationToken cancellationToken)
    {
        var command = new UpdateIngredientMinStockThresholdCommand(id, body.MinStockThreshold);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
 
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/activate")]
    public async Task<ActionResult> ActivateIngredient([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateIngredientCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
 
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult> DeactivateIngredient([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateIngredientCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
 
    /// <summary>Yeni bir alış partisi (lot) kaydeder: miktar + o anki fiyat.</summary>
    [Authorize(Roles = "ADMIN,STOCK_MANAGER,INVENTORY_STAFF")]
    [HttpPost("{id}/purchases")]
    public async Task<ActionResult> RecordPurchase([FromRoute] Guid id, RecordIngredientPurchaseRequest body, CancellationToken cancellationToken)
    {
        var command = new RecordIngredientPurchaseCommand(id, body.Quantity, body.UnitPrice, body.Note);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
 
    /// <summary>Siparişten bağımsız, elle stok düşümü (fire, zayiat, sipariş dışı elle tüketim).</summary>
    [Authorize(Roles = "ADMIN,STOCK_MANAGER,INVENTORY_STAFF")]
    [HttpPost("{id}/manual-deduction")]
    public async Task<ActionResult> RecordManualDeduction([FromRoute] Guid id, RecordManualDeductionRequest body, CancellationToken cancellationToken)
    {
        var command = new RecordManualIngredientDeductionCommand(id, body.Quantity, body.Note);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
 
    /// <summary>Sipariş dışında üretilen bir ürünün reçetesine göre ham madde stoklarını düşer.</summary>
    [Authorize(Roles = "ADMIN,STOCK_MANAGER,INVENTORY_STAFF")]
    [HttpPost("production")]
    public async Task<ActionResult> RecordProductProduction(RecordProductProductionRequest body, CancellationToken cancellationToken)
    {
        var command = new RecordProductProductionCommand(body.ProductId, body.LocationId, body.Quantity, body.Note);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
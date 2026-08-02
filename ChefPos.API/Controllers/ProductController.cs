using ChefPos.Application.Orders.Commands.RemoveOrderItem;
using ChefPos.Application.Products.Commands.ActivateProduct;
using ChefPos.Application.Products.Commands.AddProductIngredient;
using ChefPos.Application.Products.Commands.CreateProduct;
using ChefPos.Application.Products.Commands.DeactivateProduct;
using ChefPos.Application.Products.Commands.RemoveProductIngredient;
using ChefPos.Application.Products.Commands.UpdatePrice;
using ChefPos.Application.Products.Commands.UpdateProduct;
using ChefPos.Application.Products.DTOs;
using ChefPos.Application.Products.Queries.GetProductById;
using ChefPos.Application.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> CreateProduct(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateProduct(Guid id,UpdateProductRequestDto body, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(id,body.Name ,body.Description, body.ImageUrl);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpPatch("{id}/price")]
    public async Task<ActionResult> UpdateProductPrice(Guid id,UpdateProductPriceRequestDto body, CancellationToken cancellationToken)
    {
        var command = new UpdatePriceCommand(id,body.NewPrice);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }


    [HttpPost("deactivate")]
    public async Task<ActionResult> DeactivateProduct(DeactivateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("activate")]
    public async Task<ActionResult> ActivateProduct(ActivateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("{id}/ingredients")]
    public async Task<ActionResult> AddIngredient(Guid id, AddIngredientRequest body, CancellationToken cancellationToken)
    {
        var command = new AddProductIngredientCommand(id, body.IngredientId, body.QuantityPerServing);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}/ingredients/{productItemId}")]
    public async Task<ActionResult> RemoveIngredient(Guid id, Guid productItemId, CancellationToken cancellationToken)
    {
        var command = new RemoveProductIngredientCommand(id,productItemId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
            
    [HttpGet("{id}")]
    public async Task<ActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return Ok(result);
    }
    
    [HttpGet]
    public async Task<ActionResult> GetProducts( [FromQuery] Guid locationId, [FromQuery] Guid? categoryId, [FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(locationId, categoryId, includeInactive);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
}
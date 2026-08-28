using ChefPos.Application.Common.Models;
using ChefPos.Application.Products.Commands.ActivateProduct;
using ChefPos.Application.Products.Commands.AddProductIngredient;
using ChefPos.Application.Products.Commands.AddProductLocation;
using ChefPos.Application.Products.Commands.CreateProduct;
using ChefPos.Application.Products.Commands.DeactivateProduct;
using ChefPos.Application.Products.Commands.DeleteProductImage;
using ChefPos.Application.Products.Commands.RemoveProductIngredient;
using ChefPos.Application.Products.Commands.RemoveProductLocation;
using ChefPos.Application.Products.Commands.SetProductImage;
using ChefPos.Application.Products.Commands.UpdatePrice;
using ChefPos.Application.Products.Commands.UpdateProduct;
using ChefPos.Application.Products.DTOs;
using ChefPos.Application.Products.Queries.ExportProducts;
using ChefPos.Application.Products.Queries.GetProductById;
using ChefPos.Application.Products.Queries.GetProducts;
using ChefPos.Application.Products.Queries.GetProductsPaged;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "SUPER_ADMIN")]
    [HttpPost]
    public async Task<ActionResult> CreateProduct(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateProduct(Guid id,UpdateProductRequestDto body, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand(id,body.Name ,body.Description);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/image")]
    [Consumes("multipart/form-data")] // file is not represented as json so used multipart/form-data
    [RequestSizeLimit(10 * 1024 * 1024)]  // max 10 mb image
    public async Task<ActionResult> SetProductImage(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("Yüklenecek bir dosya seçin.");
        }

        await using var stream = file.OpenReadStream();
        var uploadRequest = new FileUploadRequest
        {
            Content = stream,
            FileName = file.FileName,
            ContentType = file.ContentType,
            Length = file.Length
        };

        var command = new SetProductImageCommand(id, uploadRequest);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpDelete("{id}/image")]
    public async Task<ActionResult> DeleteProductImage(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteProductImageCommand(id), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPatch("{id}/price")]
    public async Task<ActionResult> UpdateProductPrice(Guid id,UpdateProductPriceRequestDto body, CancellationToken cancellationToken)
    {
        var command = new UpdatePriceCommand(id,body.NewPrice);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }


    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("deactivate")]
    public async Task<ActionResult> DeactivateProduct(DeactivateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("activate")]
    public async Task<ActionResult> ActivateProduct(ActivateProductCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/ingredients")]
    public async Task<ActionResult> AddIngredient(Guid id, AddIngredientRequest body, CancellationToken cancellationToken)
    {
        var command = new AddProductIngredientCommand(id, body.LocationId, body.IngredientId, body.QuantityPerServing);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpDelete("{id}/ingredients/{productItemId}")]
    public async Task<ActionResult> RemoveIngredient(Guid id, Guid productItemId, [FromQuery] Guid locationId, CancellationToken cancellationToken)
    {
        var command = new RemoveProductIngredientCommand(id, locationId, productItemId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "SUPER_ADMIN")]
    [HttpPost("{id}/locations")]
    public async Task<ActionResult> AddLocation(Guid id, AddProductLocationRequestDto body, CancellationToken cancellationToken)
    {
        var command = new AddProductLocationCommand(id, body.LocationId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "SUPER_ADMIN")]
    [HttpDelete("{id}/locations/{locationId}")]
    public async Task<ActionResult> RemoveLocation(Guid id, Guid locationId, CancellationToken cancellationToken)
    {
        var command = new RemoveProductLocationCommand(id, locationId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetProductById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> GetProducts( [FromQuery] Guid locationId, [FromQuery] Guid? categoryId, [FromQuery] bool includeInactive = false, [FromQuery] bool includeUncategorized = false, CancellationToken cancellationToken = default)
    {
        var query = new GetProductsQuery(locationId, categoryId, includeInactive, includeUncategorized);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN,WAITER,CASHIER")]
    [HttpGet("paged")]
    public async Task<ActionResult> GetProductsPaged([FromQuery] string? searchTerm, [FromQuery] Guid? locationId, [FromQuery] Guid? categoryId, [FromQuery] bool? isActive, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20,[FromQuery] bool includeUncategorized = false,CancellationToken cancellationToken = default)
    {
        var query = new GetProductsPagedQuery(searchTerm, locationId, categoryId, isActive, pageNumber, pageSize, includeUncategorized);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] string? searchTerm, [FromQuery] Guid? locationId,[FromQuery] Guid? categoryId, [FromQuery] bool? isActive, [FromQuery] bool includeUncategorized = false, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new ExportProductsQuery(searchTerm, locationId, categoryId, isActive, includeUncategorized), cancellationToken);
        return File(result.Content, ChefPos.Application.Common.Export.ExportFileResult.ContentType, result.FileName);
    }
}
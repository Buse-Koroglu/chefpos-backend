using ChefPos.Application.Products.Commands.CreateProduct;
using ChefPos.Application.Products.Commands.UpdateProduct;
using ChefPos.Application.Products.DTOs;
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
}
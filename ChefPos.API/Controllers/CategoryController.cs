using ChefPos.Application.Categories.Commands.ActivateCategory;
using ChefPos.Application.Categories.Commands.CreateCategory;
using ChefPos.Application.Categories.Commands.RemoveCategory;
using ChefPos.Application.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/category")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<ActionResult> CreateCategory(CreateCategoryCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("deactivate")]
    public async Task<ActionResult> DeactivateCategory(DeactivateCategoryCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("activate")]
    public async Task<ActionResult> ActivateCategory(ActivateCategoryCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
}
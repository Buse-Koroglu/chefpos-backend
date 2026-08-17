using ChefPos.Application.Categories.Commands.ActivateCategory;
using ChefPos.Application.Categories.Commands.CreateCategory;
using ChefPos.Application.Categories.Commands.RemoveCategory;
using ChefPos.Application.Categories.Commands.UpdateCategory;
using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Categories.Queries.GetCategories;
using ChefPos.Application.Categories.Queries.GetCategoriesAdmin;
using ChefPos.Application.Categories.Queries.GetCategoryById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/category")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<ActionResult> CreateCategory(CreateCategoryCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetCategoryById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpPost("deactivate")]
    public async Task<ActionResult> DeactivateCategory(DeactivateCategoryCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpPost("activate")]
    public async Task<ActionResult> ActivateCategory(ActivateCategoryCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> GetAllCategoriesWithLocation([FromQuery] Guid locationId,
        [FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery(locationId, includeInactive);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpGet("categories")]
    public async Task<ActionResult> GetCategoriesAdmin(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? locationId,
        [FromQuery] bool? isActive,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetCategoriesAdminQuery(searchTerm, locationId, isActive, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpPatch("{id}")]
        public async Task<ActionResult> UpdateCategory(Guid id, UpdateCategoryRequestDto body, CancellationToken cancellationToken)
        {
            var command = new UpdateCategoryCommand(id,body.Name, body.Icon!);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }
    }
    

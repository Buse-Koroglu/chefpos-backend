using ChefPos.Application.Menus.Commands.ActivateMenu;
using ChefPos.Application.Menus.Commands.AddProductToMenu;
using ChefPos.Application.Menus.Commands.CreateMenu;
using ChefPos.Application.Menus.Commands.CreateProductForMenu;
using ChefPos.Application.Menus.Commands.DeactivateMenu;
using ChefPos.Application.Menus.Commands.RemoveProductFromMenu;
using ChefPos.Application.Menus.Commands.UpdateMenu;
using ChefPos.Application.Menus.DTOs;
using ChefPos.Application.Menus.Queries.GetMenuById;
using ChefPos.Application.Menus.Queries.GetMenusByLocation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/menus")]
[Authorize]
public class MenuController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenuController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost]
    public async Task<ActionResult> CreateMenu(CreateMenuCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult> GetMenusByLocation([FromQuery] Guid locationId, [FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var query = new GetMenusByLocationQuery(locationId, includeInactive);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetMenuById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetMenuByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPatch("{id}")]
    public async Task<ActionResult> UpdateMenu(Guid id, UpdateMenuRequestDto body, CancellationToken cancellationToken)
    {
        var command = new UpdateMenuCommand(id, body.Name, body.Description);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/activate")]
    public async Task<ActionResult> ActivateMenu(Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateMenuCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult> DeactivateMenu(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateMenuCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/products")]
    public async Task<ActionResult> AddProductToMenu(Guid id, AddProductToMenuRequestDto body, CancellationToken cancellationToken)
    {
        var command = new AddProductToMenuCommand(id, body.ProductId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/products/new")]
    public async Task<ActionResult> CreateProductForMenu(Guid id, CreateProductForMenuRequestDto body, CancellationToken cancellationToken)
    {
        var command = new CreateProductForMenuCommand(id, body.Name, body.Price, body.Description);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpDelete("{id}/products/{productId}")]
    public async Task<ActionResult> RemoveProductFromMenu(Guid id, Guid productId, CancellationToken cancellationToken)
    {
        var command = new RemoveProductFromMenuCommand(id, productId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
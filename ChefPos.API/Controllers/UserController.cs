using ChefPos.Application.Users.Commands.ActivateUser;
using ChefPos.Application.Users.Commands.AddRole;
using ChefPos.Application.Users.Commands.AssignLocationAccess;
using ChefPos.Application.Users.Commands.CreateUser;
using ChefPos.Application.Users.Commands.DeactivateUser;
using ChefPos.Application.Users.Commands.RemoveRole;
using ChefPos.Application.Users.Commands.RevokeLocationAccess;
using ChefPos.Application.Users.DTOs;
using ChefPos.Application.Users.Queries.GetAdminByLocation;
using ChefPos.Application.Users.Queries.ExportUsers;
using ChefPos.Application.Users.Queries.GetAllUsers;
using ChefPos.Application.Users.Queries.GetStockManagerByLocation;
using ChefPos.Application.Users.Queries.GetUserById;
using ChefPos.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using static ChefPos.Application.Common.Export.ExportFileResult;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpGet]
    public async Task<ActionResult> GetUsers([FromQuery] string? searchTerm,[FromQuery] Role? role, [FromQuery] bool? isActive, [FromQuery] Guid? locationId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetAllUsersQuery(searchTerm, role, isActive, locationId, pageNumber, pageSize), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpGet("export")]
    public async Task<IActionResult> Export([FromQuery] string? searchTerm, [FromQuery] Role? role, [FromQuery] bool? isActive, [FromQuery] Guid? locationId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ExportUsersQuery(searchTerm, role, isActive, locationId), cancellationToken);
        return File(result.Content, ContentType, result.FileName);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUserById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN")]
    [HttpGet("stock-manager")]
    public async Task<ActionResult> GetStockManagerByLocation([FromQuery] Guid locationId, CancellationToken cancellationToken)
    {
        var query = new GetStockManagerByLocationQuery(locationId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "SUPER_ADMIN")]
    [HttpGet("admin")]
    public async Task<ActionResult> GetAdminByLocation([FromQuery] Guid locationId, CancellationToken cancellationToken)
    {
        var query = new GetAdminByLocationQuery(locationId);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/locations")]
    public async Task<ActionResult> AssignLocationAccess([FromRoute] Guid id, AssignLocationAccessRequest body, CancellationToken cancellationToken)
    {
        var command = new AssignLocationAccessCommand(id, body.LocationId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id}/locations/{locationId}")]
    public async Task<ActionResult> RevokeLocationAccess([FromRoute] Guid id, [FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        var command = new RevokeLocationAccessCommand(id, locationId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/roles")]
    public async Task<ActionResult> AddRole([FromRoute] Guid id, AddRoleRequest body, CancellationToken cancellationToken)
    {
        var command = new AddRoleCommand(id, body.Role);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
 
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpDelete("{id}/roles/{role}")]
    public async Task<ActionResult> RemoveRole([FromRoute] Guid id, [FromRoute] Role role, CancellationToken cancellationToken)
    {
        var command = new RemoveRoleCommand(id, role);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/activate")]
    public async Task<ActionResult> ActivateUser([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "ADMIN,SUPER_ADMIN")]
    [HttpPost("{id}/deactivate")]
    public async Task<ActionResult> DeactivateUser([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeactivateUserCommand(id);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
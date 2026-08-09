using ChefPos.Application.Users.Commands.AddRole;
using ChefPos.Application.Users.Commands.AssignLocationAccess;
using ChefPos.Application.Users.Commands.CreateUser;
using ChefPos.Application.Users.Commands.RemoveRole;
using ChefPos.Application.Users.DTOs;
using ChefPos.Application.Users.Queries.GetAllUsers;
using ChefPos.Application.Users.Queries.GetUserById;
using ChefPos.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpPost]
    public async Task<ActionResult> CreateUser(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpGet]
    public async Task<ActionResult> GetUsers([FromQuery] Role? role, [FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery(role, includeInactive);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
 
    [Authorize(Roles = "ADMIN")]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUserById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }
    

    [HttpPost("{id}/locations")]
    public async Task<ActionResult> AssignLocationAccess([FromRoute] Guid id, AssignLocationAccessRequest body, CancellationToken cancellationToken)
    {
        var command = new AssignLocationAccessCommand(id, body.LocationId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "ADMIN")]
    [HttpPost("{id}/roles")]
    public async Task<ActionResult> AddRole([FromRoute] Guid id, AddRoleRequest body, CancellationToken cancellationToken)
    {
        var command = new AddRoleCommand(id, body.Role);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
 
    [Authorize(Roles = "ADMIN")]
    [HttpDelete("{id}/roles/{role}")]
    public async Task<ActionResult> RemoveRole([FromRoute] Guid id, [FromRoute] Role role, CancellationToken cancellationToken)
    {
        var command = new RemoveRoleCommand(id, role);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
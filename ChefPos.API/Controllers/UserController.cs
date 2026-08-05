using ChefPos.Application.Users.Commands.AssignLocationAccess;
using ChefPos.Application.Users.Commands.CreateUser;
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
    

    [HttpPost("{id}/locations")]
    public async Task<ActionResult> AssignLocationAccess([FromRoute] Guid id, AssignLocationAccessRequest body, CancellationToken cancellationToken)
    {
        var command = new AssignLocationAccessCommand(id, body.LocationId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}
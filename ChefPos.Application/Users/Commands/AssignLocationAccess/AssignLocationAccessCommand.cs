using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Users.Commands.AssignLocationAccess;

public class AssignLocationAccessCommand : IRequest<UserResponseDto>
{
    public Guid UserId { get; set; }
    public Guid LocationId { get; set; }
 
    public AssignLocationAccessCommand(Guid userId, Guid locationId)
    {
        UserId = userId;
        LocationId = locationId;
    }
}
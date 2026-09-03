using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Users.Commands.GrantRoleAtLocation;

public class GrantRoleAtLocationCommand : IRequest<UserResponseDto>
{
    public Guid UserId { get; set; }
    public Role Role { get; set; }
    public Guid LocationId { get; set; }

    public GrantRoleAtLocationCommand(Guid userId, Role role, Guid locationId)
    {
        UserId = userId;
        Role = role;
        LocationId = locationId;
    }
}

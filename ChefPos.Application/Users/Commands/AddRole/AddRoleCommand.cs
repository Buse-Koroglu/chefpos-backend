using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Users.Commands.AddRole;

public class AddRoleCommand : IRequest<UserResponseDto>
{
    public Guid UserId { get; set; }
    public Role Role { get; set; }
 
    public AddRoleCommand(Guid userId, Role role)
    {
        UserId = userId;
        Role = role;
    }
}


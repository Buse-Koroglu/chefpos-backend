using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Users.Commands.RemoveRole;

public class RemoveRoleCommand : IRequest<UserResponseDto>
{
    public Guid UserId { get; set; }
    public Role Role { get; set; }

    public RemoveRoleCommand(Guid userId, Role role)
    {
        UserId = userId;
        Role = role;
    }
}
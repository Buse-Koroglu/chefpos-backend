using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Users.Commands.DeactivateUser;

public class DeactivateUserCommand : IRequest<UserResponseDto>
{
    public Guid UserId { get; }

    public DeactivateUserCommand(Guid userId)
    {
        UserId = userId;
    }
}
using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Users.Commands.ActivateUser;

public class ActivateUserCommand : IRequest<UserResponseDto>
{
    public Guid UserId { get; }

    public ActivateUserCommand(Guid userId)
    {
        UserId = userId;
    }
}
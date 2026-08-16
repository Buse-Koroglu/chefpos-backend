using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Users.Commands.RevokeLocationAccess;

public class RevokeLocationAccessCommand : IRequest<UserResponseDto>
{
    public Guid UserId { get; }
    public Guid LocationId { get; }

    public RevokeLocationAccessCommand(Guid userId, Guid locationId)
    {
        UserId = userId;
        LocationId = locationId;
    }
}
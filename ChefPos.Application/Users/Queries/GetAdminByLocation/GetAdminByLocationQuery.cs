using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Users.Queries.GetAdminByLocation;

public class GetAdminByLocationQuery : IRequest<UserResponseDto?>
{
    public Guid LocationId { get; set; }

    public GetAdminByLocationQuery(Guid locationId)
    {
        LocationId = locationId;
    }
}

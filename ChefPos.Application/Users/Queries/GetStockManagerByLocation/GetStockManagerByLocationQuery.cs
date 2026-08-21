using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Users.Queries.GetStockManagerByLocation;

public class GetStockManagerByLocationQuery : IRequest<UserResponseDto?>
{
    public Guid LocationId { get; set; }

    public GetStockManagerByLocationQuery(Guid locationId)
    {
        LocationId = locationId;
    }
}

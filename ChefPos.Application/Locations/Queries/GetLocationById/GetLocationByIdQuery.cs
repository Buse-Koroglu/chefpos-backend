using ChefPos.Application.Locations.DTOs;
using MediatR;

public class GetLocationByIdQuery : IRequest<LocationResponseDto>
{
    public Guid LocationId { get; set; }

    public GetLocationByIdQuery(Guid locationId)
    {
        LocationId = locationId;
    }
}
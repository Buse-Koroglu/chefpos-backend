using ChefPos.Application.Locations.DTOs;
using MediatR;

namespace ChefPos.Application.Locations.Commands.ActivateLocation;

public class ActivateLocationCommand : IRequest<LocationResponseDto>
{
    public Guid LocationId { get; set; }

    public ActivateLocationCommand(Guid locationId)
    {
        LocationId = locationId;
    }
}

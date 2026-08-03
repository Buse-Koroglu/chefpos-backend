using ChefPos.Application.Locations.DTOs;
using MediatR;

namespace ChefPos.Application.Locations.Commands.DeactivateLocation;

public class DeactivateLocationCommand : IRequest<LocationResponseDto>
{
    public Guid LocationId { get; set; }

    public DeactivateLocationCommand(Guid locationId)
    {
        LocationId = locationId;
    }
}

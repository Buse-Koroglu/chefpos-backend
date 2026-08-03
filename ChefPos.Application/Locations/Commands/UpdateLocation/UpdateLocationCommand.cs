using ChefPos.Application.Locations.DTOs;
using MediatR;

namespace ChefPos.Application.Locations.Commands.UpdateLocation;

public class UpdateLocationCommand : IRequest<LocationResponseDto>
{
    public Guid LocationId { get; set; }
    public string Name { get; set; } = default!;

    public UpdateLocationCommand(Guid locationId, string name)
    {
        LocationId = locationId;
        Name = name;
    }
}
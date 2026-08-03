using ChefPos.Application.Locations.DTOs;
using MediatR;

namespace ChefPos.Application.Locations.Queries.GetLocations;

public class GetLocationsQuery : IRequest<List<LocationResponseDto>>
{
    public bool IncludeInactive { get; set; }

    public GetLocationsQuery(bool includeInactive)
    {
        IncludeInactive = includeInactive;
    }
}
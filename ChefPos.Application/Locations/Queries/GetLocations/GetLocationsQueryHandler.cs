using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Locations.DTOs;
using MediatR;

namespace ChefPos.Application.Locations.Queries.GetLocations;

public class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, List<LocationResponseDto>>
{
    private readonly ILocationRepository _locationRepository;

    public GetLocationsQueryHandler(ILocationRepository locationRepository) => _locationRepository = locationRepository;

    public async Task<List<LocationResponseDto>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        var locations = await _locationRepository.GetAllAsync(request.IncludeInactive, cancellationToken);

        return locations.Select(LocationResponseDto.FromEntity).ToList();
    }
}
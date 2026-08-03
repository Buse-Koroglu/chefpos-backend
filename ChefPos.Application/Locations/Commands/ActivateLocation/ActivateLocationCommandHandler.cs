using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Locations.DTOs;
using MediatR;

namespace ChefPos.Application.Locations.Commands.ActivateLocation;

public class ActivateLocationCommandHandler : IRequestHandler<ActivateLocationCommand, LocationResponseDto>
{
    private readonly ILocationRepository _locationRepository;

    public ActivateLocationCommandHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<LocationResponseDto> Handle(ActivateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken)
            .OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        location.ActivateLocation();

        await _locationRepository.SaveAllChangesAsync(cancellationToken);

        return LocationResponseDto.FromEntity(location);
    }
}

using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Locations.DTOs;
using MediatR;

namespace ChefPos.Application.Locations.Commands.DeactivateLocation;

public class DeactivateLocationCommandHandler : IRequestHandler<DeactivateLocationCommand, LocationResponseDto>
{
    private readonly ILocationRepository _locationRepository;

    public DeactivateLocationCommandHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<LocationResponseDto> Handle(DeactivateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken).OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        location.DeactivateLocation();

        await _locationRepository.SaveAllChangesAsync(cancellationToken);

        return LocationResponseDto.FromEntity(location);
    }
}
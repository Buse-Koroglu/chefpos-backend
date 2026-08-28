using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Locations.DTOs;
using MediatR;

namespace ChefPos.Application.Locations.Commands.UpdateLocation;


public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, LocationResponseDto>
{
    private readonly ILocationRepository _locationRepository;

    public UpdateLocationCommandHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<LocationResponseDto> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken).OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        location.Rename(request.Name);

        await _locationRepository.SaveAllChangesAsync(cancellationToken);

        return LocationResponseDto.FromEntity(location);
    }
}
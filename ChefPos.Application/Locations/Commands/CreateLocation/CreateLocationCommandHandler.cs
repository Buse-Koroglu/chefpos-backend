using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Locations.DTOs;
using ChefPos.Domain.Entities;
using MediatR;

namespace ChefPos.Application.Locations.Commands.CreateLocation;

public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, LocationResponseDto>
{
    private readonly ILocationRepository _locationRepository;

    public CreateLocationCommandHandler(ILocationRepository locationRepository)
    {
        _locationRepository = locationRepository;
    }

    public async Task<LocationResponseDto> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = new Location(request.Name);

        await _locationRepository.AddAsync(location, cancellationToken);
        await _locationRepository.SaveAllChangesAsync(cancellationToken);

        return LocationResponseDto.FromEntity(location);
    }
}
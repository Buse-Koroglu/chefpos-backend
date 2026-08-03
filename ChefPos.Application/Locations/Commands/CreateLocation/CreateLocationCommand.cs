using ChefPos.Application.Locations.DTOs;
using MediatR;

namespace ChefPos.Application.Locations.Commands.CreateLocation;

public class CreateLocationCommand : IRequest<LocationResponseDto>
{
    public string Name { get; set; } = default!;
}
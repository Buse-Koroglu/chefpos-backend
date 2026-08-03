using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.Commands.AssignLocationAccess;
using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

public class AssignLocationAccessCommandHandler : IRequestHandler<AssignLocationAccessCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILocationRepository _locationRepository;

    public AssignLocationAccessCommandHandler(IUserRepository userRepository, ILocationRepository locationRepository)
    {
        _userRepository = userRepository;
        _locationRepository = locationRepository;
    }

    public async Task<UserResponseDto> Handle(AssignLocationAccessCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {request.UserId}");

        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken)
            .OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        if (user.Role == Role.STOCK_MANAGER)
        {
            var existingManager = await _userRepository.GetStockManagerByLocationAsync(request.LocationId, cancellationToken);
            if (existingManager is not null && existingManager.Id != user.Id)
            {
                throw new InvalidOperationException(
                    $"Bu şubede zaten bir Yerleşke Stok Yetkilisi atanmış: {existingManager.FirstName} {existingManager.LastName}.");
            }
        }

        user.GiveLocationAccess(request.LocationId);

        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}
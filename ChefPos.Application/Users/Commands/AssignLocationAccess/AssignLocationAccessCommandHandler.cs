using ChefPos.Application.Common.Exceptions;
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
    private readonly ICurrentUserService _currentUserService;

    public AssignLocationAccessCommandHandler(IUserRepository userRepository, ILocationRepository locationRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _locationRepository = locationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<UserResponseDto> Handle(AssignLocationAccessCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {request.UserId}");

        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken)
            .OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        if (user.HasRole(Role.ADMIN) || user.HasRole(Role.SUPER_ADMIN))
        {
            var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
                .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

            if (!actingUser.HasRole(Role.SUPER_ADMIN))
            {
                throw new ValidationException("Bu rolü yalnızca süper yönetici atayabilir.");
            }
        }

        if (user.HasRole(Role.STOCK_MANAGER))
        {
            var existingManager = await _userRepository.GetStockManagerByLocationAsync(request.LocationId, cancellationToken);
            if (existingManager is not null && existingManager.Id != user.Id)
            {
                throw new ValidationException(
                    $"Bu şubede zaten bir Yerleşke Stok Yetkilisi atanmış: {existingManager.FirstName} {existingManager.LastName}.",
                    "STOCK_MANAGER_CONFLICT");
            }
        }

        if (user.HasRole(Role.ADMIN))
        {
            var existingAdmin = await _userRepository.GetAdminByLocationAsync(request.LocationId, cancellationToken);
            if (existingAdmin is not null && existingAdmin.Id != user.Id)
            {
                throw new ValidationException(
                    $"Bu şubede zaten bir Yönetici atanmış: {existingAdmin.FirstName} {existingAdmin.LastName}.");
            }
        }

        user.GiveLocationAccess(request.LocationId);

        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}
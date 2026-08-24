using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Menus.DTOs;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Menus.Commands.CreateMenu;

public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuResponseDto>
{
    private readonly IMenuRepository _menuRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateMenuCommandHandler(
        IMenuRepository menuRepository,
        ILocationRepository locationRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _menuRepository = menuRepository;
        _locationRepository = locationRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<MenuResponseDto> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken)
            .OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasAccessToLocation(request.LocationId))
        {
            throw new ValidationException("Bu yerleşke için işlem yapma yetkiniz yok.");
        }

        var menu = new Menu(request.Name, request.LocationId, request.Description);

        await _menuRepository.AddAsync(menu, cancellationToken);
        await _menuRepository.SaveAllChangesAsync(cancellationToken);

        return MenuResponseDto.FromEntity(menu);
    }
}
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Menus.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Menus.Commands.DeactivateMenu;

public class DeactivateMenuCommandHandler : IRequestHandler<DeactivateMenuCommand, MenuResponseDto>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeactivateMenuCommandHandler(IMenuRepository menuRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _menuRepository = menuRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<MenuResponseDto> Handle(DeactivateMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await _menuRepository.GetByIdAsync(request.MenuId, cancellationToken).OrThrowNotFoundAsync("Menü bulunamadı.");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasRoleAtLocation(Role.ADMIN, menu.LocationId))
            throw new ValidationException("Bu yerleşke için işlem yapma yetkiniz yok.");

        menu.Deactivate();
        await _menuRepository.SaveAllChangesAsync(cancellationToken);
        return MenuResponseDto.FromEntity(menu);
    }
}
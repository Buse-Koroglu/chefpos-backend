using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Menus.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Menus.Commands.AddProductToMenu;

public class AddProductToMenuCommandHandler : IRequestHandler<AddProductToMenuCommand, MenuResponseDto>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddProductToMenuCommandHandler(IMenuRepository menuRepository, IProductRepository productRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _menuRepository = menuRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<MenuResponseDto> Handle(AddProductToMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await _menuRepository.GetByIdAsync(request.MenuId, cancellationToken).OrThrowNotFoundAsync("Menü bulunamadı.");

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).OrThrowNotFoundAsync("Ürün bulunamadı.");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasAccessToLocation(menu.LocationId))
            throw new ValidationException("Bu yerleşke için işlem yapma yetkiniz yok.");

        if (!product.BelongsToLocation(menu.LocationId))
            throw new ValidationException("Ürün bu yerleşkede tanımlı değil.");

        menu.AddProduct(product.Id);
        await _menuRepository.SaveAllChangesAsync(cancellationToken);

        var refreshedMenu = await _menuRepository.GetByIdAsync(menu.Id, cancellationToken).OrThrowNotFoundAsync("Menü bulunamadı.");

        return MenuResponseDto.FromEntity(refreshedMenu);
    }
}

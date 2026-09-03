using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Menus.Commands.CreateProductForMenu;

public class CreateProductForMenuCommandHandler : IRequestHandler<CreateProductForMenuCommand, ProductResponseDto>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public CreateProductForMenuCommandHandler(IMenuRepository menuRepository, IProductRepository productRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _menuRepository = menuRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ProductResponseDto> Handle(CreateProductForMenuCommand request, CancellationToken cancellationToken)
    {
        var menu = await _menuRepository.GetByIdAsync(request.MenuId, cancellationToken).OrThrowNotFoundAsync("Menü bulunamadı.");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasRoleAtLocation(Role.ADMIN, menu.LocationId))
            throw new ValidationException("Bu yerleşke için işlem yapma yetkiniz yok.");
        
        var product = new Product(request.Name, request.Price, categoryId: null, new List<Guid> { menu.LocationId }, request.Description);
        await _productRepository.AddAsync(product, cancellationToken);
        menu.AddProduct(product.Id);
        await _productRepository.SaveAllChangesAsync(cancellationToken);

        return ProductResponseDto.FromEntity(product);
    }
}
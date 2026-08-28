using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Products.Commands.UpdatePrice;

public class UpdatePriceCommandHandler : IRequestHandler<UpdatePriceCommand,ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public  UpdatePriceCommandHandler(IProductRepository productRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ProductResponseDto> Handle(UpdatePriceCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken).OrThrowNotFoundAsync($"Ürün bulunamadı: {request.Id}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !product.ProductLocations.Any(pl => actingUser.HasAccessToLocation(pl.LocationId)))
        {
            throw new ValidationException("Bu ürünü yönetme yetkiniz yok.");
        }

        product.UpdatePrice(request.NewPrice);
        await _productRepository.SaveAllChangesAsync(cancellationToken);
        return ProductResponseDto.FromEntity(product);
    }
}
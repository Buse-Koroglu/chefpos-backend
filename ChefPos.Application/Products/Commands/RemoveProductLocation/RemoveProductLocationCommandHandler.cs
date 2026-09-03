using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Products.Commands.RemoveProductLocation;

public class RemoveProductLocationCommandHandler : IRequestHandler<RemoveProductLocationCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public RemoveProductLocationCommandHandler(IProductRepository productRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ProductResponseDto> Handle(RemoveProductLocationCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).OrThrowNotFoundAsync($"Ürün bulunamadı: {request.ProductId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasRoleAtLocation(Role.ADMIN, request.LocationId))
        {
            throw new ValidationException("Bu yerleşke için işlem yapma yetkiniz yok.");
        }

        product.RemoveLocation(request.LocationId);
        await _productRepository.SaveAllChangesAsync(cancellationToken);

        return ProductResponseDto.FromEntity(product);
    }
}

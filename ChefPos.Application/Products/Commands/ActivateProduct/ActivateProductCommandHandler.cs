using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Products.Commands.ActivateProduct;

public class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand,ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public ActivateProductCommandHandler(IProductRepository productRepository, ILocationRepository locationRepository,IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _locationRepository = locationRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<ProductResponseDto> Handle(ActivateProductCommand request, CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken).OrThrowNotFoundAsync($"Yerleşke bulunamadı : {request.LocationId}");

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken).OrThrowNotFoundAsync($"Ürün bulunamadı : {request.Id}");

        if (!product.BelongsToLocation(request.LocationId)) throw new ForbiddenException("Bu işleme yetkiniz bulunmamaktır.");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasRoleAtLocation(Role.ADMIN, request.LocationId))
            throw new ForbiddenException("Bu işleme yetkiniz bulunmamaktır.");
        
        product.ActivateProduct();
        await _productRepository.SaveAllChangesAsync(cancellationToken);
        return ProductResponseDto.FromEntity(product);
    }

}
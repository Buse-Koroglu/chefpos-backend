using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Products.Commands.DeleteProductImage;

public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public DeleteProductImageCommandHandler(
        IProductRepository productRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<ProductResponseDto> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            .OrThrowNotFoundAsync($"Ürün bulunamadı: {request.ProductId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !product.ProductLocations.Any(pl => actingUser.HasAccessToLocation(pl.LocationId)))
        {
            throw new ValidationException("Bu ürünü yönetme yetkiniz yok.");
        }

        var previousImagePath = product.ImageUrl;

        product.SetImage(null);
        await _productRepository.SaveAllChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(previousImagePath))
        {
            await _fileStorageService.DeleteAsync(previousImagePath, cancellationToken);
        }

        return ProductResponseDto.FromEntity(product);
    }
}

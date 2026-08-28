using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Settings;
using ChefPos.Application.Products.DTOs;
using ChefPos.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace ChefPos.Application.Products.Commands.SetProductImage;

public class SetProductImageCommandHandler : IRequestHandler<SetProductImageCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;
    private readonly FileStorageSettings _settings;

    public SetProductImageCommandHandler(IProductRepository productRepository, IUserRepository userRepository, ICurrentUserService currentUserService, IFileStorageService fileStorageService, IOptions<FileStorageSettings> settings)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
        _settings = settings.Value;
    }

    public async Task<ProductResponseDto> Handle(SetProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken).OrThrowNotFoundAsync($"Ürün bulunamadı: {request.ProductId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !product.ProductLocations.Any(pl => actingUser.HasAccessToLocation(pl.LocationId)))
        {
            throw new ValidationException("Bu ürünü yönetme yetkiniz yok.");
        }

        if (request.File.Length <= 0)
        {
            throw new ValidationException("Yüklenen dosya boş olamaz.");
        }

        if (request.File.Length > _settings.MaxFileSizeBytes)
        {
            throw new ValidationException($"Dosya boyutu {_settings.MaxFileSizeBytes / (1024 * 1024)}MB sınırını aşıyor.");
        }

        if (!_settings.AllowedContentTypes.Contains(request.File.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException("Desteklenmeyen dosya formatı. İzin verilenler: jpg, png, webp.");
        }

        var extension = Path.GetExtension(request.File.FileName);
        if (string.IsNullOrEmpty(extension) || !_settings.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException("Desteklenmeyen dosya uzantısı. İzin verilenler: .jpg, .jpeg, .png, .webp.");
        }

        var previousImagePath = product.ImageUrl;

        var newImagePath = await _fileStorageService.SaveImageAsync(request.File, "products", cancellationToken);

        product.SetImage(newImagePath);
        await _productRepository.SaveAllChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(previousImagePath) && previousImagePath != newImagePath)
        {
            await _fileStorageService.DeleteAsync(previousImagePath, cancellationToken);
        }

        return ProductResponseDto.FromEntity(product);
    }
}

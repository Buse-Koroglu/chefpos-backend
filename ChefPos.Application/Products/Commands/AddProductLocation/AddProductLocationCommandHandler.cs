using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.AddProductLocation;

public class AddProductLocationCommandHandler : IRequestHandler<AddProductLocationCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ICategoryRepository _categoryRepository;

    public AddProductLocationCommandHandler(IProductRepository productRepository, ILocationRepository locationRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _locationRepository = locationRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<ProductResponseDto> Handle(AddProductLocationCommand request, CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken)
            .OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            .OrThrowNotFoundAsync($"Ürün bulunamadı: {request.ProductId}");

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId, cancellationToken)
            .OrThrowNotFoundAsync($"Kategori bulunamadı: {product.CategoryId}");

        if (!category.BelongsToLocation(request.LocationId))
        {
            throw new ValidationException("Ürünün kategorisi, bu yerleşkede tanımlı değil.");
        }

        product.AddLocation(request.LocationId);
        await _productRepository.SaveAllChangesAsync(cancellationToken);

        return ProductResponseDto.FromEntity(product);
    }
}

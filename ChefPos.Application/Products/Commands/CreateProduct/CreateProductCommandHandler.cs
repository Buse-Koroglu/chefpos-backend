using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using ChefPos.Domain.Entities;
using MediatR;

namespace ChefPos.Application.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand,ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository  _categoryRepository;
    private readonly ILocationRepository _locationRepository;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ILocationRepository locationRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<ProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (request.LocationIds is null || request.LocationIds.Count == 0)
        {
            throw new ValidationException("En az bir yerleşke seçilmelidir.");
        }

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken).OrThrowNotFoundAsync($"Kategori bulunamadı: {request.CategoryId}");

        if (!category.IsActive)
        {
            throw new ValidationException("Pasif bir kategoriye ürün eklenemez.");
        }

        foreach (var locationId in request.LocationIds.Distinct())
        {
            await _locationRepository.GetByIdAsync(locationId, cancellationToken).OrThrowNotFoundAsync($"Yerleşke bulunamadı: {locationId}");

            if (!category.BelongsToLocation(locationId))
                throw new ValidationException("Seçilen kategori, seçilen yerleşkelerin tümünde tanımlı değil.");
        }

        var product = new Product(request.Name, request.Price, request.CategoryId, request.LocationIds,
            request.Description);

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveAllChangesAsync(cancellationToken);
        return ProductResponseDto.FromEntity(product);

    }

}
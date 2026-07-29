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

    public CreateProductCommandHandler(IProductRepository productRepository, ICategoryRepository categoryRepository,ILocationRepository locationRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<ProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        if (location is null)
        {
            throw new KeyNotFoundException("Yerleşke bulunamadı.");
        }

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            throw new KeyNotFoundException("Kategori bulunamadı.");
        }
                
        if (!category.IsActive)
        {
            throw new InvalidOperationException("Pasif bir kategoriye ürün eklenemez.");
        }

        if (category.LocationId != request.LocationId)
        {
            throw new InvalidOperationException("Kategori bu yerleşkeye ait değil.");
        }

        var product = new Product(request.Name, request.Price, request.CategoryId, request.LocationId,
            request.Description, request.ImageUrl);

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveAllChangesAsync(cancellationToken);
        return ProductResponseDto.FromEntity(product);

    }
    
}
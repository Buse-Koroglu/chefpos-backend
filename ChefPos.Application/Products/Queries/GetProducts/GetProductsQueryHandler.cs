using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Queries.GetProducts.GetProductsQueryHandler;


public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductResponseDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ICategoryRepository _categoryRepository;

    public GetProductsQueryHandler(IProductRepository productRepository, ILocationRepository locationRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _locationRepository = locationRepository;
        _categoryRepository = categoryRepository;
    }
 
    public async Task<List<ProductResponseDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        if (request.CategoryId.HasValue)
        {
            await _categoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken).OrThrowNotFoundAsync($"Kategori bulunamadı: {request.CategoryId}");
        }
        await _locationRepository.GetByIdAsync(request.LocationId,cancellationToken).OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        var products = await _productRepository.GetAllByLocationAsync(
            request.LocationId,
            request.CategoryId,
            request.IncludeInactive,
            request.IncludeUncategorized,
            cancellationToken);
        
 
        return products.Select(ProductResponseDto.FromEntity).ToList();
    }
}
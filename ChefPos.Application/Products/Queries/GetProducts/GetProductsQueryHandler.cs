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
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category is null)
            {
                throw new KeyNotFoundException($"Kategori bulunamadı: {request.CategoryId.Value}");
            }
        }
        var location = await _locationRepository.GetByIdAsync(request.LocationId,cancellationToken);
        if (location is null)
        {
            throw new KeyNotFoundException("Yerleşke bulunamadı.");
        }

        var products = await _productRepository.GetAllByLocationAsync(
            request.LocationId,
            request.CategoryId,
            request.IncludeInactive,
            cancellationToken);
        
 
        return products.Select(ProductResponseDto.FromEntity).ToList();
    }
}
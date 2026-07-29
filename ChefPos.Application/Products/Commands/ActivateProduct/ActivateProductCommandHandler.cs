using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.ActivateProduct;

public class ActivateProductCommandHandler : IRequestHandler<ActivateProductCommand,ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ILocationRepository _locationRepository;
    
    public ActivateProductCommandHandler(IProductRepository productRepository, ILocationRepository locationRepository)
    {
        _productRepository = productRepository;
        _locationRepository = locationRepository;
    }

    public async Task<ProductResponseDto> Handle(ActivateProductCommand request, CancellationToken cancellationToken)
    {
        var location =await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        if (location is null)
        {
            throw new KeyNotFoundException("Yerleşke bulunamadı.");
        }

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            throw new KeyNotFoundException("Ürün bulunamadı.");
        }
        
        product.ActivateProduct();
        await _productRepository.SaveAllChangesAsync(cancellationToken);
        return ProductResponseDto.FromEntity(product);
    }
    
}
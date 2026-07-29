using ChefPos.Application.Common.Behaviors;
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
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken).OrThrowNotFoundAsync($"Yerleşke bulunamadı : {request.LocationId}");

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken).OrThrowNotFoundAsync($"Ürün bulunamadı : {request.Id}");
        
        product.ActivateProduct();
        await _productRepository.SaveAllChangesAsync(cancellationToken);
        return ProductResponseDto.FromEntity(product);
    }
    
}
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.DeactivateProduct;

public class DeactivateProductCommandHandler : IRequestHandler<DeactivateProductCommand,ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ILocationRepository _locationRepository;

    public DeactivateProductCommandHandler(IProductRepository productRepository, ILocationRepository locationRepository)
    {
        _productRepository = productRepository;
        _locationRepository = locationRepository;
    }

    public async Task<ProductResponseDto> Handle(DeactivateProductCommand request, CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken).OrThrowNotFoundAsync($"Yerleşke bulunmadı: {request.LocationId}");

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken).OrThrowNotFoundAsync($"Ürün bulunamadı: {request.Id}");
        product.DeactivateProduct();
        await _productRepository.SaveAllChangesAsync(cancellationToken);
        return ProductResponseDto.FromEntity(product);

    }
}
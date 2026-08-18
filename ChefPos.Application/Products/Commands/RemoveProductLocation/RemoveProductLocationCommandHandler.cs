using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.RemoveProductLocation;

public class RemoveProductLocationCommandHandler : IRequestHandler<RemoveProductLocationCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;

    public RemoveProductLocationCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponseDto> Handle(RemoveProductLocationCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            .OrThrowNotFoundAsync($"Ürün bulunamadı: {request.ProductId}");

        product.RemoveLocation(request.LocationId);
        await _productRepository.SaveAllChangesAsync(cancellationToken);

        return ProductResponseDto.FromEntity(product);
    }
}

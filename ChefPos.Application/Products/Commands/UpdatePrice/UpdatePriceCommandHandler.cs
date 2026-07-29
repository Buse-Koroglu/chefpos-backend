using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.UpdatePrice;

public class UpdatePriceCommandHandler : IRequestHandler<UpdatePriceCommand,ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    
    public  UpdatePriceCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponseDto> Handle(UpdatePriceCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            throw new KeyNotFoundException("Ürün bulunamadı.");
        }
        product.UpdatePrice(request.NewPrice);
        await _productRepository.SaveAllChangesAsync(cancellationToken);
        return ProductResponseDto.FromEntity(product);
    }
}
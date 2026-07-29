using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand,ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    
    public  UpdateProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponseDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new KeyNotFoundException("Ürün bulunamadı.");
        }
        product.UpdateDetails(request.Name,request.Description,request.ImageUrl);
        await _productRepository.SaveAllChangesAsync(cancellationToken);
        return ProductResponseDto.FromEntity(product);
    }
}
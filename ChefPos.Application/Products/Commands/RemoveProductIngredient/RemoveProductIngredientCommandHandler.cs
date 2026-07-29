using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.RemoveProductIngredient;

public class RemoveProductIngredientCommandHandler : IRequestHandler<RemoveProductIngredientCommand,ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    
    public  RemoveProductIngredientCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponseDto> Handle(RemoveProductIngredientCommand request, CancellationToken cancellationToken)
    {
        var product =await _productRepository.GetByIdAsync(request.ProductId,cancellationToken);
        if (product is null)
        {
            throw new KeyNotFoundException("Ürün bulunamadı.");
        }
        product.RemoveIngredient(request.IngredientId);
        await _productRepository.SaveAllChangesAsync(cancellationToken);
        return ProductResponseDto.FromEntity(product);
    }
    
    
}
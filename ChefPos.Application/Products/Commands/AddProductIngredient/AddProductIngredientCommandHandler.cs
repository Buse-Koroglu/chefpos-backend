using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.AddProductIngredient;

public class AddProductIngredientCommandHandler : IRequestHandler<AddProductIngredientCommand,ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    
    public AddProductIngredientCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponseDto> Handle(AddProductIngredientCommand request, CancellationToken cancellationToken)
    {
        var product =await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new KeyNotFoundException("Ürün bulunamadı.");
        }
        product.AddIngredient(request.Name,request.UnitPrice);
        await _productRepository.SaveAllChangesAsync(cancellationToken);
        return ProductResponseDto.FromEntity(product);
    }
    
}
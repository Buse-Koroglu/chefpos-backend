using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.AddProductIngredient;

public class AddProductIngredientCommandHandler : IRequestHandler<AddProductIngredientCommand, ProductResponseDto>
{
    private readonly IProductRepository _productRepository;
    private readonly IIngredientRepository _ingredientRepository;

    public AddProductIngredientCommandHandler(IProductRepository productRepository, IIngredientRepository ingredientRepository)
    {
        _productRepository = productRepository;
        _ingredientRepository = ingredientRepository;
    }

    public async Task<ProductResponseDto> Handle(AddProductIngredientCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new KeyNotFoundException("Ürün bulunamadı.");
        }

        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken);
        if (ingredient is null)
        {
            throw new KeyNotFoundException("Ham madde bulunamadı.");
        }

        if (ingredient.LocationId != product.LocationId)
        {
            throw new InvalidOperationException("Ham madde bu ürünün yerleşkesine ait değil.");
        }

        product.AddIngredient(request.IngredientId, request.QuantityPerServing);

        await _productRepository.SaveAllChangesAsync(cancellationToken);

        return ProductResponseDto.FromEntity(product);
    }
}
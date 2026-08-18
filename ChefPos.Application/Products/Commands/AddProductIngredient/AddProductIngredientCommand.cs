namespace ChefPos.Application.Products.Commands.AddProductIngredient;

using ChefPos.Application.Products.DTOs;
using MediatR;

public class AddProductIngredientCommand : IRequest<ProductResponseDto>
{
    public Guid ProductId { get; set; }
    public Guid LocationId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal QuantityPerServing { get; set; }

    public AddProductIngredientCommand(Guid productId, Guid locationId, Guid ingredientId, decimal quantityPerServing)
    {
        ProductId = productId;
        LocationId = locationId;
        IngredientId = ingredientId;
        QuantityPerServing = quantityPerServing;
    }
}

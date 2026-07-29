using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.RemoveProductIngredient;

public class RemoveProductIngredientCommand : IRequest<ProductResponseDto>
{
    public Guid ProductId { get; set; }
    public Guid IngredientId { get; set; }

    public RemoveProductIngredientCommand(Guid productId, Guid ingredientId)
    {
        ProductId = productId;
        IngredientId = ingredientId;
    }
    
}
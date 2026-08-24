using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.UpdateIngredientPrice;

public class UpdateIngredientPriceCommand : IRequest<IngredientResponseDto>
{
    public Guid IngredientId { get; set; }
    public decimal UnitPrice { get; set; }

    public UpdateIngredientPriceCommand(Guid ingredientId, decimal unitPrice)
    {
        IngredientId = ingredientId;
        UnitPrice = unitPrice;
    }
}

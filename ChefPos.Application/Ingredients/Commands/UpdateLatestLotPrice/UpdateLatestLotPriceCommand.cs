using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.UpdateIngredientPrice;

public class UpdateLatestLotPriceCommand : IRequest<IngredientResponseDto>
{
    public Guid IngredientId { get; set; }
    public decimal UnitPrice { get; set; }

    public UpdateLatestLotPriceCommand(Guid ingredientId, decimal unitPrice)
    {
        IngredientId = ingredientId;
        UnitPrice = unitPrice;
    }
}

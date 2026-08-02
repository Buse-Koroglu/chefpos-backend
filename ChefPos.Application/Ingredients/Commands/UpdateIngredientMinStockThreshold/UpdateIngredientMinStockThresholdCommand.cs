using ChefPos.Application.Ingredients.DTOs;

namespace ChefPos.Application.Ingredients.Commands.UpdateIngredientMinStockThreshold;

using MediatR;

public class UpdateIngredientMinStockThresholdCommand : IRequest<IngredientResponseDto>
{
    public Guid IngredientId { get; set; }
    public decimal MinStockThreshold { get; set; }

    public UpdateIngredientMinStockThresholdCommand(Guid ingredientId, decimal minStockThreshold)
    {
        IngredientId = ingredientId;
        MinStockThreshold = minStockThreshold;
    }
}
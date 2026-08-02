using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.DeactivateIngredient;

public class DeactivateIngredientCommand : IRequest<IngredientResponseDto>
{
    public Guid IngredientId { get; set; }

    public DeactivateIngredientCommand(Guid ingredientId)
    {
        IngredientId = ingredientId;
    }
}
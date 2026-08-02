using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.ActivateIngredient;

public class ActivateIngredientCommand : IRequest<IngredientResponseDto>
{
    public Guid IngredientId { get; set; }

    public ActivateIngredientCommand(Guid ingredientId)
    {
        IngredientId = ingredientId;
    }
}
using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.UpdateIngredient;

public class UpdateIngredientCommand : IRequest<IngredientResponseDto>
{
    public Guid IngredientId { get; set; }
    public string Name { get; set; } = default!;
    public decimal UnitPrice { get; set; }
 
    public UpdateIngredientCommand(Guid ingredientId, string name, decimal unitPrice)
    {
        IngredientId = ingredientId;
        Name = name;
        UnitPrice = unitPrice;
    }
}
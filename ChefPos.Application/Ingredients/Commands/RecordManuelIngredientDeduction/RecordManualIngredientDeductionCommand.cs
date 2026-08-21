using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.RecordManuelIngredientDeduction;

public class RecordManualIngredientDeductionCommand : IRequest<IngredientResponseDto>
{
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string Note { get; set; } = default!;

    public RecordManualIngredientDeductionCommand(Guid ingredientId, decimal quantity, string note)
    {
        IngredientId = ingredientId;
        Quantity = quantity;
        Note = note;
    }
}
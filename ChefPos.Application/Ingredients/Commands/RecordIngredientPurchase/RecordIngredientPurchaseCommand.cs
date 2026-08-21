using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.RecordIngredientPurchase;

public class RecordIngredientPurchaseCommand : IRequest<IngredientResponseDto>
{
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Note { get; set; }

    public RecordIngredientPurchaseCommand(Guid ingredientId, decimal quantity, decimal unitPrice, string? note = null)
    {
        IngredientId = ingredientId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Note = note;
    }
}
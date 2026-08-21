using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.RecordProductProduction;

public class RecordProductProductionCommand : IRequest<List<IngredientResponseDto>>
{
    public Guid ProductId { get; set; }
    public Guid LocationId { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }

    public RecordProductProductionCommand(Guid productId, Guid locationId, int quantity, string? note = null)
    {
        ProductId = productId;
        LocationId = locationId;
        Quantity = quantity;
        Note = note;
    }
}
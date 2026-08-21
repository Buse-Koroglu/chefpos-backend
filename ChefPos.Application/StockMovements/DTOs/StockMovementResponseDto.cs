using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.StockMovements.DTOs;

public class StockMovementResponseDto
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = default!;
    public StockUnit Unit { get; set; }

    public Guid LocationId { get; set; }

    public StockMovementType Type { get; set; }
    public decimal Quantity { get; set; }

    public Guid PerformedByUserId { get; set; }
    public string PerformedByUserName { get; set; } = default!;

    public Guid? RelatedOrderId { get; set; }
    public Guid? RelatedProductId { get; set; }

    public string? Note { get; set; }
    public decimal WeightedUnitPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public static StockMovementResponseDto FromEntity(StockMovement movement)
    {
        return new StockMovementResponseDto
        {
            Id = movement.Id,
            IngredientId = movement.IngredientId,
            IngredientName = movement.Ingredient?.Name ?? string.Empty,
            Unit = movement.Ingredient?.Unit ?? default(StockUnit),
            LocationId = movement.LocationId,
            Type = movement.Type,
            Quantity = movement.Quantity,
            PerformedByUserId = movement.PerformedByUserId,
            PerformedByUserName = movement.PerformedByUser != null
                ? $"{movement.PerformedByUser.FirstName} {movement.PerformedByUser.LastName}".Trim()
                : string.Empty,
            RelatedOrderId = movement.RelatedOrderId,
            RelatedProductId = movement.RelatedProductId,
            Note = movement.Note,
            WeightedUnitPrice = movement.WeightedUnitPrice,
            CreatedAt = movement.CreatedAt
        };
    }
}

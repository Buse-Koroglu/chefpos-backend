using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class StockMovement : BaseEntity
{
    public Guid IngredientId { get; private set; }
    public Ingredient Ingredient { get; private set; } = null!;

    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;

    public StockMovementType Type { get; private set; }
    public decimal Quantity { get; private set; }

    public Guid PerformedByUserId { get; private set; }
    public User PerformedByUser { get; private set; } = null!;

    public Guid? RelatedOrderId { get; private set; }

    public Guid? RelatedProductId { get; private set; }

    public string? Note { get; private set; }

    private readonly List<StockMovementLotConsumption> _lotConsumptions = new();
    public IReadOnlyCollection<StockMovementLotConsumption> LotConsumptions => _lotConsumptions;

    public decimal WeightedUnitPrice =>
        Quantity <= 0
            ? 0
            : _lotConsumptions.Sum(c => c.QuantityConsumed * c.UnitPriceAtConsumption) / Quantity;

    private StockMovement() { }

    private StockMovement(
        Guid ingredientId,
        Guid locationId,
        StockMovementType type,
        decimal quantity,
        Guid performedByUserId,
        Guid? relatedOrderId,
        Guid? relatedProductId,
        string? note)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Hareket miktarı pozitif olmalı.");

        IngredientId = ingredientId;
        LocationId = locationId;
        Type = type;
        Quantity = quantity;
        PerformedByUserId = performedByUserId;
        RelatedOrderId = relatedOrderId;
        RelatedProductId = relatedProductId;
        Note = note;
    }

    public static StockMovement CreatePurchase(Guid ingredientId, Guid locationId, decimal quantity, Guid performedByUserId, IngredientLot lot, string? note = null)
    {
        var movement = new StockMovement(ingredientId, locationId, StockMovementType.PURCHASE, quantity, performedByUserId, null, null, note);
        movement._lotConsumptions.Add(new StockMovementLotConsumption(movement.Id, lot.Id, quantity, lot.UnitPrice));
        return movement;
    }

    public static StockMovement CreateOrderSaleDeduction(Guid ingredientId, Guid locationId, Guid performedByUserId, Guid orderId, IReadOnlyList<(IngredientLot Lot, decimal Quantity)> consumptions)
    {
        var total = consumptions.Sum(c => c.Quantity);
        var movement = new StockMovement(ingredientId, locationId, StockMovementType.ORDER_SALE, total, performedByUserId, orderId, null, null);
        foreach (var (lot, qty) in consumptions)
            movement._lotConsumptions.Add(new StockMovementLotConsumption(movement.Id, lot.Id, qty, lot.UnitPrice));
        return movement;
    }

    public static StockMovement CreateManualDeduction(Guid ingredientId, Guid locationId, Guid performedByUserId, IReadOnlyList<(IngredientLot Lot, decimal Quantity)> consumptions, string? note)
    {
        var total = consumptions.Sum(c => c.Quantity);
        var movement = new StockMovement(ingredientId, locationId, StockMovementType.MANUAL_DEDUCTION, total, performedByUserId, null, null, note);
        foreach (var (lot, qty) in consumptions)
            movement._lotConsumptions.Add(new StockMovementLotConsumption(movement.Id, lot.Id, qty, lot.UnitPrice));
        return movement;
    }

    public static StockMovement CreateProductionDeduction(Guid ingredientId, Guid locationId, Guid performedByUserId, Guid producedProductId, IReadOnlyList<(IngredientLot Lot, decimal Quantity)> consumptions, string? note)
    {
        var total = consumptions.Sum(c => c.Quantity);
        var movement = new StockMovement(ingredientId, locationId, StockMovementType.PRODUCTION_DEDUCTION, total, performedByUserId, null, producedProductId, note);
        foreach (var (lot, qty) in consumptions)
            movement._lotConsumptions.Add(new StockMovementLotConsumption(movement.Id, lot.Id, qty, lot.UnitPrice));
        return movement;
    }
}
using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class StockMovementLotConsumption : BaseEntity
{
    public Guid StockMovementId { get; private set; }
    public StockMovement StockMovement { get; private set; } = null!;

    public Guid IngredientLotId { get; private set; }
    public IngredientLot IngredientLot { get; private set; } = null!;

    public decimal QuantityConsumed { get; private set; }
    public decimal UnitPriceAtConsumption { get; private set; }

    private StockMovementLotConsumption() { }

    internal StockMovementLotConsumption(Guid stockMovementId, Guid ingredientLotId, decimal quantityConsumed, decimal unitPriceAtConsumption)
    {
        if (quantityConsumed <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantityConsumed), "Tüketilen miktar pozitif olmalı.");

        StockMovementId = stockMovementId;
        IngredientLotId = ingredientLotId;
        QuantityConsumed = quantityConsumed;
        UnitPriceAtConsumption = unitPriceAtConsumption;
    }
}
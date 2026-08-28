using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class StockMovementLotConsumption : BaseEntity // bir stok movement sonrası hangi partiden ne kadar düşüş oldu kayıtlarını tutar
{
    public Guid StockMovementId { get; private set; }
    public StockMovement StockMovement { get; private set; } = null!;
    public Guid IngredientLotId { get; private set; }
    public IngredientLot IngredientLot { get; private set; } = null!;
    public decimal QuantityConsumed { get; private set; } // o partiden ne kadar tüketildiği
    public decimal UnitPriceAtConsumption { get; private set; } // o partiden tüketim yapıldığındaki birim fiyat

    private StockMovementLotConsumption() { }

    internal StockMovementLotConsumption(Guid stockMovementId, Guid ingredientLotId, decimal quantityConsumed, decimal unitPriceAtConsumption)
    {
        if (quantityConsumed <= 0) throw new ArgumentOutOfRangeException(nameof(quantityConsumed), "Tüketilen miktar pozitif olmalı.");

        StockMovementId = stockMovementId;
        IngredientLotId = ingredientLotId;
        QuantityConsumed = quantityConsumed;
        UnitPriceAtConsumption = unitPriceAtConsumption;
    }
}
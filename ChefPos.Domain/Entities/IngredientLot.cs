using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class IngredientLot : BaseEntity
{
    public Guid IngredientId { get; private set; }
    public Ingredient Ingredient { get; private set; } = null!;

    public decimal InitialQuantity { get; private set; }
    public decimal RemainingQuantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public DateTime PurchasedAt { get; private set; }

    public Guid? SourceStockRequestId { get; private set; }

    private IngredientLot() { }

    public IngredientLot(Guid ingredientId, decimal quantity, decimal unitPrice, Guid? sourceStockRequestId = null, DateTime? purchasedAt = null)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Parti miktarı pozitif olmalı.");
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Fiyat negatif olamaz.");

        IngredientId = ingredientId;
        InitialQuantity = quantity;
        RemainingQuantity = quantity;
        UnitPrice = unitPrice;
        SourceStockRequestId = sourceStockRequestId;
        PurchasedAt = purchasedAt ?? DateTime.UtcNow;
    }

    public bool IsDepleted => RemainingQuantity <= 0;

    internal decimal Consume(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Düşülecek miktar pozitif olmalı.");

        var consumed = Math.Min(amount, RemainingQuantity);
        RemainingQuantity -= consumed;
        Touch();
        return consumed;
    }

    internal void UpdateUnitPrice(decimal newUnitPrice)
    {
        if (newUnitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(newUnitPrice), "Fiyat negatif olamaz.");

        UnitPrice = newUnitPrice;
        Touch();
    }
}
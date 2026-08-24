using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class Ingredient : BaseEntity
{
    public string Name { get; private set; } = default!;
    public StockUnit Unit { get; private set; }
    public decimal CurrentStock { get; private set; }
    public decimal MinStockThreshold { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;

    private readonly List<ProductItem> _productItems = new();
    public IReadOnlyCollection<ProductItem> ProductItems => _productItems;

    private readonly List<IngredientLot> _lots = new();
    public IReadOnlyCollection<IngredientLot> Lots => _lots;

    public decimal? LatestUnitPrice => _lots.OrderByDescending(l => l.PurchasedAt).FirstOrDefault()?.UnitPrice;

    public decimal WeightedAverageUnitPrice
    {
        get
        {
            var remaining = _lots.Where(l => l.RemainingQuantity > 0).ToList();
            var totalQty = remaining.Sum(l => l.RemainingQuantity);
            return totalQty <= 0 ? 0 : remaining.Sum(l => l.RemainingQuantity * l.UnitPrice) / totalQty;
        }
    }

    private Ingredient(){}

    public Ingredient(string name, StockUnit unit, decimal initialUnitPrice, Guid locationId, decimal initialStock = 0,
        decimal minStockThreshold = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Ham madde adı boş olamaz.", nameof(name));
        }

        if (initialUnitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialUnitPrice), "Fiyat negatif olamaz.");
        }

        if (initialStock < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(initialStock), "Stok negatif olamaz.");
        }

        if (minStockThreshold < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minStockThreshold), "Minimum stok eşiği negatif olamaz.");
        }

        Name = name;
        Unit = unit;
        LocationId = locationId;
        MinStockThreshold = minStockThreshold;

        if (initialStock > 0)
        {
            AddPurchaseLot(initialStock, initialUnitPrice);
        }
    }

    public void UpdateDetails(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Ham madde adı boş olamaz.", nameof(name));
        }
        Name = name;
        Touch();
    }

    public void UpdateMinStockThreshold(decimal minStockThreshold)
    {
        if (minStockThreshold < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minStockThreshold), "Minimum stok eşiği negatif olamaz.");
        }
        MinStockThreshold = minStockThreshold;
        Touch();
    }

    public IngredientLot AddPurchaseLot(decimal quantity, decimal unitPrice, Guid? sourceStockRequestId = null, DateTime? purchasedAt = null)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Eklenecek miktar pozitif olmalı.");
        }
        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Fiyat negatif olamaz.");
        }

        var lot = new IngredientLot(Id, quantity, unitPrice, sourceStockRequestId, purchasedAt);
        _lots.Add(lot);
        CurrentStock += quantity;
        Touch();
        return lot;
    }
    
    public void UpdateLatestPurchasePrice(decimal newUnitPrice)
    {
        var latestLot = _lots.OrderByDescending(l => l.PurchasedAt).FirstOrDefault();
        if (latestLot is null)
        {
            throw new InvalidOperationException($"'{Name}' için düzenlenecek bir alış kaydı bulunmuyor.");
        }

        latestLot.UpdateUnitPrice(newUnitPrice);
        Touch();
    }

    public IReadOnlyList<(IngredientLot Lot, decimal Quantity)> DeductStockFifo(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Düşülecek miktar pozitif olmalı.");
        }

        if (CurrentStock - amount < 0)
        {
            throw new InvalidOperationException(
                $"Yetersiz stok: '{Name}' için mevcut stok {CurrentStock}, talep edilen {amount}.");
        }

        var remainingToDeduct = amount;
        var consumptions = new List<(IngredientLot Lot, decimal Quantity)>();

        foreach (var lot in _lots.Where(l => l.RemainingQuantity > 0).OrderBy(l => l.PurchasedAt))
        {
            if (remainingToDeduct <= 0) break;

            var consumed = lot.Consume(remainingToDeduct);
            if (consumed > 0)
            {
                consumptions.Add((lot, consumed));
                remainingToDeduct -= consumed;
            }
        }

        CurrentStock -= amount;
        Touch();
        return consumptions;
    }

    public bool IsBellowThreshold => CurrentStock < MinStockThreshold;

    public void DeactivateIngredient()
    {
        IsActive = false;
        Touch();
    }

    public void ActivateIngredient()
    {
        IsActive = true;
        Touch();
    }

}
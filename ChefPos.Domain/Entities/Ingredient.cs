using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class Ingredient : BaseEntity
{
    public string Name { get; private set; } = default!;
    public StockUnit Unit { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal CurrentStock { get; private set; }
    public decimal MinStockThreshold { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;

    private readonly List<ProductItem> _productItems = new();
    public IReadOnlyCollection<ProductItem> ProductItems => _productItems;
    
    private Ingredient(){}

    public Ingredient(string name, StockUnit unit, decimal unitPrice, Guid locationId, decimal initialStock = 0,
        decimal minStockThreshold = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Ham madde adı boş olamaz.", nameof(name));
        }
 
        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Fiyat negatif olamaz.");
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
        UnitPrice = unitPrice;
        LocationId = locationId;
        CurrentStock = initialStock;
        MinStockThreshold = minStockThreshold;
    }

    public void UpdateDetails(string name, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Ham madde adı boş olamaz.", nameof(name));
        }
 
        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Fiyat negatif olamaz.");
        }
        Name = name;
        UnitPrice = unitPrice;
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

    public void IncreaseStock(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Eklenecek miktar pozitif olmalı.");
        }
        CurrentStock += amount;
        Touch();
    }

    public void DecreaseStock(decimal amount)
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

        CurrentStock -= amount;
        Touch();
    }
        
    public bool IsBellowThreshold => CurrentStock > MinStockThreshold;

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
using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid? CategoryId { get; private set; }
    public Category? Category { get; private set; }

    private readonly List<ProductLocation> _locations = new();
    public IReadOnlyCollection<ProductLocation> ProductLocations => _locations;
    public IEnumerable<Guid> LocationIds => _locations.Select(l => l.LocationId);

    private Product() { }

    public Product(string name, decimal price, Guid? categoryId, IEnumerable<Guid> locationIds, string? description = null, string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Ürün adı boş olamaz.", nameof(name));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Fiyat negatif olamaz.");
        }

        var distinctLocationIds = locationIds?.Distinct().ToList() ?? new List<Guid>();
        if (distinctLocationIds.Count == 0)
        {
            throw new ArgumentException("Ürüne en az bir yerleşke atanmalıdır.", nameof(locationIds));
        }

        Name = name;
        Price = price;
        Description = description;
        ImageUrl = imageUrl;
        CategoryId = categoryId;
        foreach (var locationId in distinctLocationIds)
            _locations.Add(new ProductLocation(Id, locationId));
    }

    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(newPrice), "Fiyat negatif olamaz.");
        Price = newPrice;
        Touch();
    }

    public bool BelongsToLocation(Guid locationId) => _locations.Any(l => l.LocationId == locationId);

    public void AddLocation(Guid locationId)
    {
        if (_locations.Any(l => l.LocationId == locationId))
            throw new InvalidOperationException("Ürün zaten bu yerleşkede tanımlı.");
        _locations.Add(new ProductLocation(Id, locationId));
        Touch();
    }

    public void RemoveLocation(Guid locationId)
    {
        if (_locations.Count <= 1)
            throw new InvalidOperationException("Ürünün en az bir yerleşkesi olmalıdır.");

        var link = _locations.FirstOrDefault(l => l.LocationId == locationId);
        if (link is null) return;

        _locations.Remove(link);
        Touch();
    }

    private ProductLocation GetLocationOrThrow(Guid locationId)
    {
        var location = _locations.FirstOrDefault(l => l.LocationId == locationId);
        if (location is null)
        {
            throw new InvalidOperationException("Ürün bu yerleşkede tanımlı değil.");
        }
        return location;
    }

    public void AddIngredient(Guid locationId, Guid ingredientId, decimal quantityPerServing)
    {
        GetLocationOrThrow(locationId).AddIngredient(ingredientId, quantityPerServing);
        Touch();
    }

    public void UpdateIngredientQuantity(Guid locationId, Guid productItemId, decimal newQuantityPerServing)
    {
        GetLocationOrThrow(locationId).UpdateIngredientQuantity(productItemId, newQuantityPerServing);
        Touch();
    }

    public void RemoveIngredient(Guid locationId, Guid productItemId)
    {
        GetLocationOrThrow(locationId).RemoveIngredient(productItemId);
        Touch();
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Ürün adı boş olamaz.", nameof(name));
        }
        Name = name;
        Description = description;
        Touch();
    }

    public void SetImage(string? imagePath)
    {
        ImageUrl = imagePath;
        Touch();
    }

    public void DeactivateProduct() { IsActive = false; Touch(); }
    public void ActivateProduct() { IsActive = true; Touch(); }
}

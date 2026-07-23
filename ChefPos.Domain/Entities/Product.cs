using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public ICollection<ProductItem> ProductItems { get; private set; } = new List<ProductItem>();
    
    private Product() { }

    public Product(string name, decimal price, Guid categoryId , Guid locationId,string? description = null, string? imageUrl = null )
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Ürün adı boş olamaz.", nameof(name));
        }

        if (price < 0)
        {
            throw new ArgumentOutOfRangeException("Fiyat negatif olamaz.", nameof(price));
        } 
       Name = name;
       Price = price;
       Description = description;
       ImageUrl = imageUrl;
       LocationId = locationId;
       CategoryId = categoryId;
    }
    
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(newPrice), "Fiyat negatif olamaz.");
        Price = newPrice;
        Touch();
    }
    
    public void DeactivateProduct() { IsActive = false; Touch(); }
    public void ActivateProduct() { IsActive = true; Touch(); }
}
using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Icon { get; private set; }
    public bool IsActive { get; private set; } = true;
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    public ICollection<Product> Products { get; private set; } = new List<Product>();
    
    private Category() { }

    public Category(string categoryName, Guid locationId, string? icon = null)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("Kategori adı boş bırakılamaz.", nameof(categoryName));
        }
        Name = categoryName;
        LocationId = locationId;
        Icon = icon;
    }
    
    public void DeactivateCategory() { IsActive = false; Touch(); }
    public void ActivateCategory() { IsActive = true; Touch(); }
}
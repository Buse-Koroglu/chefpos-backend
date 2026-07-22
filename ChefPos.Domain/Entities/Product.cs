using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string? Description { get; private set; }
    public string? ImageUrl { get; private set; }
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; } = null!;
    public ICollection<ProductItem> ProductItems { get; private set; } = new List<ProductItem>();
}
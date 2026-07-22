using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string? Icon { get; private set; }
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    public ICollection<Product> Products { get; private set; } = new List<Product>();
}
using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Products : BaseEntity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string Description { get; private set; }
    public string ImageUrl { get; private set; }
    public Guid LocationId { get; private set; }
    public Locations? Location { get; private set; }
    public Guid CategoryId { get; private set; }
    public Categories? Category { get; private set; }
    public ICollection<ProductItems> ProductItems { get; private set; } = new List<ProductItems>();
}
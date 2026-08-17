using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class CategoryLocation : BaseEntity
{
    public Guid CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;

    private CategoryLocation() { }

    internal CategoryLocation(Guid categoryId, Guid locationId)
    {
        CategoryId = categoryId;
        LocationId = locationId;
    }
}
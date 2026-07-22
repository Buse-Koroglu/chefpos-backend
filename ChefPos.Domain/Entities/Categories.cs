using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Categories : BaseEntity
{
    public string Name { get; private set; }
    public string Icon { get; private set; }
    public Guid LocationId { get; private set; }
    public Locations? Location { get; private set; }
}
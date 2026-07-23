using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class UserLocation : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;
    
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    
    private UserLocation() {}
    
    internal UserLocation(Guid userId, Guid locationId)
    {
        UserId = userId;
        LocationId = locationId;
    }
        

}
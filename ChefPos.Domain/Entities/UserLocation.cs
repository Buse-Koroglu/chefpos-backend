using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class UserLocation : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public Guid LocationId { get; set; }
    public Location Location { get; set; } = null!;
    
    private UserLocation() {}
    
    internal UserLocation(Guid userId, Guid locationId)
    {
        UserId = userId;
        LocationId = locationId;
    }
        

}
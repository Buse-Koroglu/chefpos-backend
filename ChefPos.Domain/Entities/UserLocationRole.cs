using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class UserLocationRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;

    public Role Role { get; private set; }

    private UserLocationRole() { }

    internal UserLocationRole(Guid userId, Guid locationId, Role role)
    {
        UserId = userId;
        LocationId = locationId;
        Role = role;
    }
}

using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class UserRole : BaseEntity
{
    public Guid UserId { get;private set; }
    public User User { get; private set; } = null!;
    public Role Role { get; private set; }
    public UserRole(Guid userId, Role role)
    {
        UserId = userId;
        Role = role;
    }

}
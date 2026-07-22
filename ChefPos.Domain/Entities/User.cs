using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class User :  BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Password { get; private set; }
    public bool IsFirstLogin { get; set; } = true;
    public Role Role { get; private set; }
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    public ICollection<Order> Orders { get; private set; } = new List<Order>();
}
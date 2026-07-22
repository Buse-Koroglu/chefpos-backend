using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class Users :  BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Password { get; private set; }
    public bool IsFirstLogin { get; set; } = true;
    public Role Role { get; private set; }
    public Guid LocationId { get; private set; }
    public Locations Locations { get; private set; } = null!;
    public ICollection<Orders> Orders { get; private set; } = new List<Orders>();
}
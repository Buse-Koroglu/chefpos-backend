using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class User :  BaseEntity
{
    public string PersonalId { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Password { get; private set; } = default!;
    public bool IsFirstLogin { get; set; } = true;
    public Role Role { get; private set; }
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    
    public DateTime UpdatedAt { get; private set; }

    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders;
    
    private User() {}
    
    public User(string personalId, string firstName, string lastName, string password, Role role, Guid locationId)
    {
        if (string.IsNullOrWhiteSpace(personalId))
            throw new ArgumentNullException(nameof(personalId), "Personel ID boş olamaz.");

        PersonalId = personalId;
        FirstName = firstName;
        LastName = lastName;
        Password = password;
        Role = role;
        LocationId = locationId;
        IsFirstLogin = true;
    }
    
    public void ChangePassword(string newPassword)
    {
        Password = newPassword;
        IsFirstLogin = false;
    }

}
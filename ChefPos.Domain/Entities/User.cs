using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class User :  BaseEntity
{
    public string PersonalId { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string Password { get; private set; } = default!;
    public bool IsFirstLogin { get; private set; } = true;
    public Role Role { get; private set; }
    
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;
    
    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders;

    private readonly List<UserLocation> _locations = new();
    public IReadOnlyCollection<UserLocation> Locations => _locations;
    
    private User() { }

    public User(string personalId, string firstName, string lastName,
        string password, Role role, Guid LocationId)
    {
        if (string.IsNullOrWhiteSpace(personalId))
            throw new ArgumentException("Personel ID boş olamaz.", nameof(personalId));
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Ad/Soyad boş olamaz.");

        PersonalId = personalId;
        FirstName = firstName;
        LastName = lastName;
        Password = password;
        Role = role;
        IsFirstLogin = true;
        IsActive = true;

        _locations.Add(new UserLocation(Id,LocationId));
    }

    public void ChangePassword(string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Şifre boş olamaz.", nameof(newPassword));

        Password = newPassword;
        IsFirstLogin = false;
        Touch();
    }

    public void GiveLocationAccess(Guid locationId)
    {
        if (_locations.Any(l => l.LocationId == locationId))
            throw new InvalidOperationException("Kullanıcı zaten bu yerleşkede yetkilidir.");
        _locations.Add(new UserLocation(Id, locationId));
        Touch();
    }

    public void RevokeLocationAccess(Guid locationId)
    {
        if (_locations.Count == 1)
            throw new InvalidOperationException("Kullanıcının en az bir yetkili yerleşkesi olmalı.");

        var link = _locations.FirstOrDefault(l => l.LocationId == locationId);
        if (link is null) return;

        _locations.Remove(link);
        Touch();
    }

    public bool HasAccessToLocation(Guid locationId) => _locations.Any(l => l.LocationId == locationId);

    public void DeactivateUser() { IsActive = false; Touch(); }
    public void ActivateUser() { IsActive = true; Touch(); }

}
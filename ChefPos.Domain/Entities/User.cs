using System.Runtime.InteropServices.ComTypes;
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
    
    private readonly List<UserRole> _roles = new(); // bir personel birden fazla UserRole'a sahip olabilir.
    public IReadOnlyCollection<UserRole> UserRoles => _roles;
    public IEnumerable<Role> Roles => _roles.Select(r => r.Role);
    public bool IsActive { get; private set; } = true;
    
    private readonly List<Order> _orders = new();
    public IReadOnlyCollection<Order> Orders => _orders;

    private readonly List<UserLocation> _locations = new(); // bir personel birden fazla lokasyonda bulunabilir.
    public IReadOnlyCollection<UserLocation> Locations => _locations;
    
    private User() { }

    public User(string personalId, string firstName, string lastName,string password, IEnumerable<Role>roles)
    {
        if (string.IsNullOrWhiteSpace(personalId))
            throw new ArgumentException("Personel ID boş olamaz.", nameof(personalId));
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Ad/Soyad boş olamaz.");
        if (personalId.Trim().Length != 11 || !personalId.Trim().All(char.IsDigit))
            throw new ArgumentException("Personel ID 11 haneli, sadece rakamlardan oluşmalı.", nameof(personalId));
        var distinctRoles = roles?.Distinct().ToList() ?? new List<Role>();  // roles?.Distinct().ToList() null ise boş bir liste oluşturur.
        if (distinctRoles.Count == 0)
            throw new ArgumentException("Kullanıcıya en az bir rol atanmalıdır.", nameof(roles));
        
        PersonalId = personalId;
        FirstName = firstName;
        LastName = lastName;
        Password = password;
        foreach (var role in distinctRoles)
        {
            _roles.Add(new UserRole(Id, role));
        }
        IsFirstLogin = true;
        IsActive = true;
    }

    public void ChangePassword(string newPassword){
        if (string.IsNullOrWhiteSpace(newPassword)) throw new ArgumentException("Şifre boş olamaz.", nameof(newPassword));
        Password = newPassword;
        IsFirstLogin = false;
        Touch();
    }

    public void GiveLocationAccess(Guid locationId)
    {
        if (_locations.Any(l => l.LocationId == locationId)) throw new InvalidOperationException("Kullanıcı zaten bu yerleşkede yetkilidir.");
        _locations.Add(new UserLocation(Id, locationId));
        Touch();
    }

    public void RevokeLocationAccess(Guid locationId)
    {
        var location = _locations.FirstOrDefault(x => x.LocationId == locationId);
        if (location is null)
            return;
        _locations.Remove(location);
        Touch();
    }

    public bool HasAccessToLocation(Guid locationId) => _locations.Any(l => l.LocationId == locationId);

    public void DeactivateUser() { IsActive = false; Touch(); }
    public void ActivateUser() { IsActive = true; Touch(); }
    
    public bool HasRole(Role role) => _roles.Any(r => r.Role == role);
 
    public void AddRole(Role role)
    {
        if (HasRole(role)) return;
        _roles.Add(new UserRole(Id, role));
        Touch();
    }
 
    public void RemoveRole(Role role)
    {
        if (_roles.Count <= 1) throw new InvalidOperationException("Kullanıcının en az bir rolü olmalıdır.");
        var removedRole = _roles.FirstOrDefault(r => r.Role == role);
        if (removedRole is null) return;
        _roles.Remove(removedRole);
        Touch();
    }

}
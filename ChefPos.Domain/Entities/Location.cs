using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Location : BaseEntity
{
    public string Name { get; private set; } = default!;
    public bool IsActive { get; private set; } = true;
    public ICollection<Order> Orders { get; private set; } = new List<Order>();
    public ICollection<Product> Products { get; private set; } = new List<Product>();
    public ICollection<UserLocation> AuthorizedUsers { get; private set; } = new List<UserLocation>();
    public ICollection<Category> Categories { get; private set; } = new List<Category>();
    
    private Location() {}

    public Location(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
        }
        Name = name;
    }
    
    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException($"'{nameof(newName)}' cannot be null or whitespace.", nameof(newName));
        }
        Name = newName;
        Touch();
    }
    
    public void DeactivateLocation() { IsActive = false; Touch(); }
    public void ActivateLocation() { IsActive = true; Touch(); }
    
}
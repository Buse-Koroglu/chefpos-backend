using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Icon { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<CategoryLocation> _locations = new();
    public IReadOnlyCollection<CategoryLocation> CategoryLocations => _locations;
    public IEnumerable<Guid> LocationIds => _locations.Select(l => l.LocationId);

    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { }

    public Category(string categoryName, IEnumerable<Guid> locationIds, string? icon = null)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            throw new ArgumentException("Kategori adı boş bırakılamaz.", nameof(categoryName));
        }

        var distinctLocationIds = locationIds?.Distinct().ToList() ?? new List<Guid>();
        if (distinctLocationIds.Count == 0)
        {
            throw new ArgumentException("Kategoriye en az bir yerleşke atanmalıdır.", nameof(locationIds));
        }

        Name = categoryName;
        Icon = icon;
        foreach (var locationId in distinctLocationIds)
            _locations.Add(new CategoryLocation(Id, locationId));
    }

    public void UpdateDetails(string name, string? icon)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Kategori adı boş bırakılamaz.", nameof(name));
        }
        Name = name;
        Icon = icon;
        Touch();
    }

    public void AddLocation(Guid locationId)
    {
        if (_locations.Any(l => l.LocationId == locationId))
            throw new InvalidOperationException("Kategori zaten bu yerleşkede tanımlı.");
        _locations.Add(new CategoryLocation(Id, locationId));
        Touch();
    }

    public void RemoveLocation(Guid locationId)
    {
        if (_locations.Count <= 1)
            throw new InvalidOperationException("Kategorinin en az bir yerleşkesi olmalıdır.");

        var link = _locations.FirstOrDefault(l => l.LocationId == locationId);
        if (link is null) return;

        _locations.Remove(link);
        Touch();
    }

    public bool BelongsToLocation(Guid locationId) => _locations.Any(l => l.LocationId == locationId);

    public void DeactivateCategory() { IsActive = false; Touch(); }
    public void ActivateCategory() { IsActive = true; Touch(); }
}
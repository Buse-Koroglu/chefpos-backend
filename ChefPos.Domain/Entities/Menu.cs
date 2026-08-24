using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Menu : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;

    private readonly List<MenuProduct> _menuProducts = new();
    public IReadOnlyCollection<MenuProduct> MenuProducts => _menuProducts;

    private Menu() { }

    public Menu(string name, Guid locationId, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Menü adı boş olamaz.", nameof(name));
        }

        Name = name;
        LocationId = locationId;
        Description = description;
        IsActive = true;
    }

    public void AddProduct(Guid productId)
    {
        if (_menuProducts.Any(mp => mp.ProductId == productId))
        {
            throw new InvalidOperationException("Bu ürün menüde zaten mevcut.");
        }

        _menuProducts.Add(new MenuProduct(Id, productId, _menuProducts.Count));
        Touch();
    }

    public void RemoveProduct(Guid productId)
    {
        var item = _menuProducts.FirstOrDefault(mp => mp.ProductId == productId);
        if (item is null)
        {
            throw new KeyNotFoundException("Bu ürün menüde bulunamadı.");
        }

        _menuProducts.Remove(item);
        Touch();
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Menü adı boş olamaz.", nameof(name));
        }

        Name = name;
        Description = description;
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
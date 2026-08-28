using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class ProductLocation : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;

    private readonly List<ProductItem> _productItems = new();
    public IReadOnlyCollection<ProductItem> ProductItems => _productItems;

    private ProductLocation() { }

    internal ProductLocation(Guid productId, Guid locationId)
    {
        ProductId = productId;
        LocationId = locationId;
    }

    internal void AddIngredient(Guid ingredientId, decimal quantityPerServing)
    {
        if (_productItems.Any(i => i.IngredientId == ingredientId))
        {
            throw new InvalidOperationException("Bu ham madde bu yerleşkenin reçetesinde zaten mevcut.");
        }

        _productItems.Add(new ProductItem(Id, ingredientId, quantityPerServing));
    }

    internal void UpdateIngredientQuantity(Guid productItemId, decimal newQuantityPerServing)
    {
        var item = _productItems.FirstOrDefault(i => i.Id == productItemId);
        if (item is null)
        {
            throw new KeyNotFoundException("Ham madde bu yerleşkenin reçetesinde bulunamadı.");
        }
        item.UpdateQuantity(newQuantityPerServing);
    }

    internal void RemoveIngredient(Guid productItemId)
    {
        var item = _productItems.FirstOrDefault(i => i.Id == productItemId);
        if (item is null)
        {
            throw new KeyNotFoundException("Ham madde bu yerleşkenin reçetesinde bulunamadı.");
        }

        _productItems.Remove(item);
    }
}

using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class ProductItem : BaseEntity
{
    public string Name { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    
    private  ProductItem()
    {}

    public ProductItem(Guid productId, string name, decimal unitPrice)
    {
    
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Ham madde adı boş olamaz.", nameof(name));
        }

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException("Fiyat negatif olamaz.", nameof(unitPrice));
        } 
        
        Name = name;
        UnitPrice = unitPrice;
        ProductId = productId;
    
    }
}
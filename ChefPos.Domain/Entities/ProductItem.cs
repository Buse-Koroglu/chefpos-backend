using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class ProductItem : BaseEntity
{
    public string Name { get; private set; }
    public decimal UnitPrice { get; private set; }
    
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
}
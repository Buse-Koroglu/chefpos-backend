using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class ProductItems : BaseEntity
{
    public string Name { get; private set; }
    public decimal UnitPrice { get; private set; }
    public Guid ProductId { get; private set; }
    public Products? Product { get; private set; }
}
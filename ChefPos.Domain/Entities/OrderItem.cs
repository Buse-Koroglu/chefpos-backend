using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Order Order { get; private set; } = null!;
    public Guid? ProductId { get; private set; }
    public Product? Product { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int  Quantity { get; private set; }
}
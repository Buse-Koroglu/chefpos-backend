using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class OrderItems : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Orders Order { get; private set; }
    public Guid ProductId { get; private set; }
    public Products Product { get; private set; }
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public int  Quantity { get; private set; }
}
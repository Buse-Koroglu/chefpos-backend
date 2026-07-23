using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; } 
    public Order Order { get; private set; } = null!;
    public Guid? ProductId { get; private set; }
    public string Name { get; private set; } = default!;
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    
    private  OrderItem()
    {
    }

    internal OrderItem(Guid productId, string name, decimal price, int quantity)
    {
        ProductId = productId;
        Name = name;
        Price = price;
        Quantity = quantity;
    }
    
    internal void IncreaseQuantity(int amount)
    {
        Quantity += amount;
    }
}
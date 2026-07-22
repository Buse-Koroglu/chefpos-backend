using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class Order : BaseEntity
{
    public int OrderNumber { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public OrderType OrderType { get; private set; }
    public string? CustomerName { get; private set; }
    public decimal TotalPrice { get; private set; }
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    public Guid? CashierId { get; private set; }
    public User? Cashier { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
}
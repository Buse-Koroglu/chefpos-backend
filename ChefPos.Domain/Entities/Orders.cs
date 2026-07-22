using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class Orders : BaseEntity
{
    public int OrderNumber { get; private set; }
    public OrderStatus OrderStatus { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public OrderType OrderType { get; private set; }
    public string CustomerName { get; private set; }
    public decimal TotalPrice { get; private set; }
    public Guid LocationId { get; private set; }
    public Locations? Location { get; private set; }
    public Guid? CashierId { get; private set; }
    public Users? Cashier { get; private set; }
    public ICollection<OrderItems> OrderItems { get; private set; } = new List<OrderItems>();
}
using ChefPos.Domain.Common;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Entities;

public class Order : BaseEntity
{
    public int OrderNumber { get; private set; }
    public OrderStatus OrderStatus { get; private set; } = OrderStatus.PENDING;
    public PaymentStatus PaymentStatus { get; private set; } = PaymentStatus.UNPAID;
    public OrderType OrderType { get; private set; }
    public string CustomerName { get; private set; } = default!;
    public decimal TotalPrice { get; private set; }
    public Guid LocationId { get; private set; }
    public Location Location { get; private set; } = null!;
    public Guid? CashierId { get; private set; }
    public User? Cashier { get; private set; }

    private readonly List<OrderItem> _items = new();
    public IReadOnlyCollection<OrderItem> Items => _items;

    Order()
    {
    }

    public static Order CreateByCashier(int orderNumber, Guid locationId, Guid cashierId, string customerName, decimal totalPrice)
    {
        return new Order
        {
            OrderNumber = orderNumber,
            CashierId = cashierId,
            LocationId = locationId,
            CustomerName = string.IsNullOrEmpty(customerName) ? "Müşteri" : customerName,
            TotalPrice = totalPrice,
            OrderType = OrderType.CASH,
        };
    }

    public static Order CreateByKiosk(int orderNumber, Guid locationId, string customerName, decimal totalPrice)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("Kiosk siparişlerinde isim-soyisim girmek zorunludur.", nameof(customerName));
        }
        return new Order
        {
            OrderNumber = orderNumber,
            CashierId = null,
            LocationId = locationId,
            CustomerName = customerName,
            TotalPrice = totalPrice,
            OrderType = OrderType.SELF_SERVICE,
        };
    }
}
        

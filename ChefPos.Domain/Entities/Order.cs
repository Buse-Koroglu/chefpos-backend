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
    
    private void CalculateTotalPrice() => TotalPrice = _items.Sum(i => i.Price * i.Quantity);


    public static Order CreateByCashier(int orderNumber, Guid locationId, Guid cashierId, string customerName)
    {
        return new Order
        {
            OrderNumber = orderNumber,
            CashierId = cashierId,
            LocationId = locationId,
            CustomerName = string.IsNullOrEmpty(customerName) ? "Müşteri" : customerName,
            OrderType = OrderType.CASHIER,
        };
    }

    public static Order CreateByKiosk(int orderNumber, Guid locationId, string customerName)
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
            OrderType = OrderType.SELF_SERVICE,
        };
    }
    
    public void AddItem(Guid productId, int quantity, decimal unitPrice, string productName)
    {
        if (OrderStatus != OrderStatus.PENDING)
        {
            throw new InvalidOperationException("Sadece bekleyen siparişler değişitirilebilir.");
        }
        
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Adet 0'dan büyük olmalı.");
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice));
        
        var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem is not null)
        {
            existingItem.IncreaseQuantity(quantity);
        }
        else
        {
            _items.Add(new OrderItem(productId,productName,unitPrice,quantity));
        }
        CalculateTotalPrice();
        Touch();
    }

    public void RemoveItem(Guid orderItemId)
    {
        if (OrderStatus != OrderStatus.PENDING)
        {
            throw new InvalidOperationException("Sadece bekleyen siparişler değişitirilebilir.");
        }
        var item = _items.FirstOrDefault(i => i.Id == orderItemId);
        if (item is null) return;
        _items.Remove(item);
        CalculateTotalPrice();
        Touch();    
    }
    
    public void Complete()
    {
        if (!_items.Any())
            throw new InvalidOperationException("Boş sipariş tamamlanamaz.");
        if (OrderStatus != OrderStatus.PENDING)
            throw new InvalidOperationException($"Sipariş tamamlandığı ve iptal edildiği durumundayken tamamlanamaz.");

        OrderStatus = OrderStatus.COMPLETED;
        Touch();
    }

    public void Cancel()
    {
        if (OrderStatus == OrderStatus.COMPLETED)
            throw new InvalidOperationException("Tamamlanmış sipariş iptal edilemez.");

        OrderStatus = OrderStatus.CANCELLED;
        Touch();
    }

    public void MarkAsPaid()
    {
        if (PaymentStatus == PaymentStatus.PAID)
            throw new InvalidOperationException("Sipariş zaten ödenmiş.");

        PaymentStatus = PaymentStatus.PAID;
        Touch();
    }

}
        

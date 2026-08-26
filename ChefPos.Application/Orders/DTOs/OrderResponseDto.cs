using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.Orders.DTOs;

public class OrderResponseDto
{
    public Guid Id { get; set; }
    public int OrderNumber { get; set; }
    public string CustomerName { get; set; } = default!;
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string PaymentStatus { get; set; } = default!;
    public List<OrderItemResponseDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid? TableId { get; set; }
    public int? TableNumber { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public bool IsPackage { get; set; }


    public static OrderResponseDto FromEntity(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerName = order.CustomerName,
            TotalPrice = order.TotalPrice,
            Status = order.OrderStatus.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            Type = order.OrderType.ToString(),
            Items = order.Items.Select(OrderItemResponseDto.FromEntity).ToList(),
            CreatedAt = order.CreatedAt,
            CompletedAt = order.CompletedAt,
            TableId = order.TableId,
            TableNumber = order.Table?.TableNumber,
            CreatedByUserId = order.CreatedByUserId,
            IsPackage = order.IsPackage
        };
    }
}

public class OrderItemResponseDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public Guid? ProductId { get; set; }
    public static OrderItemResponseDto FromEntity(OrderItem item)
    {
        return new OrderItemResponseDto
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            Quantity = item.Quantity,
            ProductId = item.ProductId
        };
    }
}
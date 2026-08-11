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
    public DateTime? CompletedAt { get; set; }
    

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
            CompletedAt = order.CompletedAt
        };
    }
}

public class OrderItemResponseDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public static OrderItemResponseDto FromEntity(OrderItem item)
    {
        return new OrderItemResponseDto
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            Quantity = item.Quantity
        };
    }
}
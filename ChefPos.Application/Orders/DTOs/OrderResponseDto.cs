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
}

public class OrderItemResponseDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
}
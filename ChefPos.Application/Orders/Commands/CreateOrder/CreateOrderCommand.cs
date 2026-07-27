using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands;

public class CreateOrderCommand : IRequest<OrderResponseDto>
{
    public Guid LocationId { get; set; }
    public Guid CashierId { get; set; }
    public string? CustomerName { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
}

public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
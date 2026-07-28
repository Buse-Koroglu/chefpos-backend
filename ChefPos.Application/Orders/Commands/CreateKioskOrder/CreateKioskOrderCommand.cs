using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.CreateKioskOrder;

public class CreateKioskOrderCommand :IRequest<OrderResponseDto>
{
    public Guid LocationId { get; set; }
    public string? CustomerName { get; set; }
    public List<CreateOrderItemDto> Items { get; set; } = new();
    
}
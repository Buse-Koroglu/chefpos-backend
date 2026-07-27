using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.CompleteOrder;

public class CompleteOrderCommand : IRequest<OrderResponseDto>
{
    public Guid OrderId { get; set; }
    
    public CompleteOrderCommand(Guid orderId)
    {
        OrderId = orderId;
    }
}
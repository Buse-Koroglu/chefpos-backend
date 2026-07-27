using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.CancelOrder;

public class CancelOrderCommand : IRequest<OrderResponseDto>
{
    public Guid OrderId { get; set; }

    public CancelOrderCommand(Guid orderId)
    {
        OrderId = orderId;
    }
}
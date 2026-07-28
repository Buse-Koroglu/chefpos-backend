using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.RemoveOrderItem;

public class RemoveOrderItemCommand : IRequest<OrderResponseDto>
{
    public Guid OrderId { get; set; }
    public Guid OrderItemId { get; set; }

    public RemoveOrderItemCommand(Guid orderId, Guid orderItemId)
    {
        OrderId = orderId;
        OrderItemId = orderItemId;
    }
}
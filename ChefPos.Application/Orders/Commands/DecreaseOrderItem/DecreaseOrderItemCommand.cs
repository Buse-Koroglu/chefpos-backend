using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.DecreaseOrderItem;

public class DecreaseOrderItemCommand : IRequest<OrderResponseDto>
{
    public Guid OrderId { get; set; }
    public Guid OrderItemId { get; set; }
    public int Quantity { get; set; }

    public DecreaseOrderItemCommand(Guid orderId, Guid orderItemId, int quantity)
    {
        OrderId = orderId;
        OrderItemId = orderItemId;
        Quantity = quantity;    
    }
}
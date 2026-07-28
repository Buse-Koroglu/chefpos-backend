using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.AddOrderItem;

public class AddOrderItemCommand : IRequest<OrderResponseDto>
{
    public Guid OrderId { get; set; }
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
    
    public AddOrderItemCommand(Guid orderId, int quantity, Guid productId)
    {
        OrderId = orderId;
        Quantity = quantity;
        ProductId = productId;
    }
    
}
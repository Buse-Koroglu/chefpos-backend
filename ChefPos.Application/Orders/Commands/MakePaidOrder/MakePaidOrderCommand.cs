using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.MakePaidOrder;

public class MakePaidOrderCommand : IRequest<OrderResponseDto>
{
    public Guid OrderId { get; set; }

    public MakePaidOrderCommand(Guid orderId)
    {
        OrderId = orderId;
    }
}
using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.MakeKioskOrderPaid;

public class MakeKioskOrderPaidCommand : IRequest<OrderResponseDto>
{
    public Guid OrderId { get; set; }

    public MakeKioskOrderPaidCommand(Guid orderId)
    {
        OrderId = orderId;
    }
}

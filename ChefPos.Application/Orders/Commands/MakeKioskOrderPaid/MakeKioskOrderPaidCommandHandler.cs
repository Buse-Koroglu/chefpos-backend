using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Orders.Commands.MakeKioskOrderPaid;

public class MakeKioskOrderPaidCommandHandler : IRequestHandler<MakeKioskOrderPaidCommand, OrderResponseDto>
{
    private readonly IOrderRepository _orderRepository;

    public MakeKioskOrderPaidCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponseDto> Handle(MakeKioskOrderPaidCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken)
            .OrThrowNotFoundAsync($"Sipariş bulunamadı: {request.OrderId}");

        if (order.OrderType != OrderType.SELF_SERVICE || order.CreatedByUserId != null)
        {
            throw new ForbiddenException("Bu işlem yalnızca kiosk siparişleri için geçerlidir.");
        }

        order.MarkAsPaid();
        await _orderRepository.SaveAllChangesAsync(cancellationToken);
        return OrderResponseDto.FromEntity(order);
    }
}

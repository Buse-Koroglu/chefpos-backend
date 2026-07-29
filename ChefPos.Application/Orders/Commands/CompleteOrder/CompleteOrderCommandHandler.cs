using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.CompleteOrder;

public class CompleteOrderCommandHandler : IRequestHandler<CompleteOrderCommand,OrderResponseDto>
{
    private readonly IOrderRepository _orderRepository;

    public CompleteOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }
    
    public async Task<OrderResponseDto> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken).OrThrowNotFoundAsync($"Sipariş bulunamadı : {request.OrderId}");
        order.Complete();
        await _orderRepository.SaveAllChangesAsync(cancellationToken);
        return OrderResponseDto.FromEntity(order);
    }
}
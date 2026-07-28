using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Commands.DecreaseOrderItem;

public class DecreaseOrderItemCommandHandler : IRequestHandler<DecreaseOrderItemCommand,OrderResponseDto>
{
    private readonly IOrderRepository _orderRepository;

    public DecreaseOrderItemCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponseDto> Handle(DecreaseOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            throw new KeyNotFoundException($"Sipariş bulunamadı: {request.OrderId}");

        }
        
        order.DecreaseQuantity(request.OrderItemId, request.Quantity);
        await _orderRepository.SaveAllChangesAsync(cancellationToken);
        return OrderResponseDto.FromEntity(order);
        
    }
}
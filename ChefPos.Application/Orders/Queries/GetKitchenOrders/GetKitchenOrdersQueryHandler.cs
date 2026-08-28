using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Enums;
using MediatR;
namespace ChefPos.Application.Orders.Queries.GetKitchenOrders;
public class GetKitchenOrdersQueryHandler : IRequestHandler<GetKitchenOrdersQuery, List<OrderResponseDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetKitchenOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<List<OrderResponseDto>> Handle(GetKitchenOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllByLocationAsync(request.LocationId, request.Status, OrderType.WAITER, cancellationToken);

        return orders.Select(OrderResponseDto.FromEntity).ToList();
    }
}
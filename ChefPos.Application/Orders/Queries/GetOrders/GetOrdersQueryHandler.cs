using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, List<OrderResponseDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILocationRepository _locationRepository;
 
    public GetOrdersQueryHandler(IOrderRepository orderRepository, ILocationRepository locationRepository)
    {
        _orderRepository = orderRepository;
        _locationRepository = locationRepository;
    }
 
    public async Task<List<OrderResponseDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken)
            .OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");
 
        var orders = await _orderRepository.GetAllByLocationAsync(request.LocationId, request.Status, cancellationToken);
 
        return orders.Select(OrderResponseDto.FromEntity).ToList();
    }
}
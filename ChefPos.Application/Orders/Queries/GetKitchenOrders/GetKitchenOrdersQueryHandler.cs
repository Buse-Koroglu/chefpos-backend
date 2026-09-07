using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Orders.DTOs;
using MediatR;
namespace ChefPos.Application.Orders.Queries.GetKitchenOrders;
public class GetKitchenOrdersQueryHandler : IRequestHandler<GetKitchenOrdersQuery, PagedResult<OrderResponseDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetKitchenOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderResponseDto>> Handle(GetKitchenOrdersQuery request, CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _orderRepository.GetKitchenOrdersPagedAsync(
            request.LocationId,
            request.Status,
            request.Types,
            request.SearchTerm,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return new PagedResult<OrderResponseDto>
        {
            Items = orders.Select(OrderResponseDto.FromEntity).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}

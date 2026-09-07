using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Orders.Queries.GetKitchenOrders;

public class GetKitchenOrdersQuery : IRequest<PagedResult<OrderResponseDto>>
{
    private static readonly OrderType[] DefaultTypes = { OrderType.WAITER, OrderType.SELF_SERVICE };

    public Guid LocationId { get; }
    public OrderStatus? Status { get; }
    public IReadOnlyList<OrderType> Types { get; }
    public string? SearchTerm { get; }
    public int PageNumber { get; }
    public int PageSize { get; }

    public GetKitchenOrdersQuery(
        Guid locationId,
        OrderStatus? status,
        IReadOnlyList<OrderType>? types,
        string? searchTerm,
        int pageNumber = 1,
        int pageSize = 20)
    {
        LocationId = locationId;
        Status = status;
        Types = types is { Count: > 0 } ? types : DefaultTypes;
        SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize is < 1 or > 100 ? 20 : pageSize;
    }
}

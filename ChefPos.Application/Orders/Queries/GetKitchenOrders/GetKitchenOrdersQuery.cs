using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Enums;
using MediatR;
namespace ChefPos.Application.Orders.Queries.GetKitchenOrders;

public class GetKitchenOrdersQuery : IRequest<List<OrderResponseDto>>
{
    public Guid LocationId { get; }
    public OrderStatus? Status { get; }

    public GetKitchenOrdersQuery(Guid locationId, OrderStatus? status)
    {
        LocationId = locationId;
        Status = status;
    }
}
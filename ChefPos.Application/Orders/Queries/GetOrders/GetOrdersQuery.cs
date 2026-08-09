using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Orders.Queries.GetOrders;

public class GetOrdersQuery : IRequest<List<OrderResponseDto>>
{
    public Guid LocationId { get; set; }
    public OrderStatus? Status { get; set; }
 
    public GetOrdersQuery(Guid locationId, OrderStatus? status)
    {
        LocationId = locationId;
        Status = status;
    }
}
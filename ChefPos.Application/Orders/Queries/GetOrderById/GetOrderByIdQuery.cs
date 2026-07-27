using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<OrderResponseDto>
{
    public Guid OrderId { get; set; }

    public GetOrderByIdQuery(Guid orderId)
    {
        OrderId = orderId;
    }
}
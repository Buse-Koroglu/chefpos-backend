using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Orders.Queries.GetOrders;

public class GetOrdersQuery : IRequest<PagedResult<OrderResponseDto>>
{
    public Guid LocationId { get; set; }
    public OrderStatus? Status { get; set; }
    
    public OrderType? Type { get; set; }
    
    public PaymentStatus? PaymentStatus { get; set; }

    public string? SearchTerm { get; }

    public Guid? CreatedByUserId { get; }

    public DateTime? FromDate { get; }

    public DateTime? ToDate { get; }

    public int PageNumber { get; }
    public int PageSize { get; }

    public GetOrdersQuery(
        Guid locationId,
        OrderStatus? status,
        OrderType? type,
        PaymentStatus? paymentStatus,
        string? searchTerm,
        Guid? createdByUserId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 20)
    {
        LocationId = locationId;
        Status = status;
        PaymentStatus = paymentStatus;
        Type = type;
        SearchTerm = string.IsNullOrWhiteSpace(searchTerm)
            ? null
            : searchTerm.Trim();
        CreatedByUserId = createdByUserId;
        FromDate = fromDate;
        ToDate = toDate;
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize is < 1 or > 100 ? 20 : pageSize;
    }
}
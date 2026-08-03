using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.StockRequests.Queries.GetStockRequests;

public class GetStockRequestsQuery : IRequest<List<StockRequestResponseDto>>
{
    public Guid LocationId { get; set; }
    public StockRequestStatus? Status { get; set; }
 
    public GetStockRequestsQuery(Guid locationId, StockRequestStatus? status)
    {
        LocationId = locationId;
        Status = status;
    }
}
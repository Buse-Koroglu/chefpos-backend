using ChefPos.Application.StockRequests.DTOs;
using MediatR;

namespace ChefPos.Application.StockRequests.Queries.GetStockRequestById;

public class GetStockRequestByIdQuery : IRequest<StockRequestResponseDto>
{
    public Guid StockRequestId { get; set; }
 
    public GetStockRequestByIdQuery(Guid stockRequestId)
    {
        StockRequestId = stockRequestId;
    }
}
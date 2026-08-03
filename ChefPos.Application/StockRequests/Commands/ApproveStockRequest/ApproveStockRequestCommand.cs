using ChefPos.Application.StockRequests.DTOs;
using MediatR;

namespace ChefPos.Application.StockRequests.Commands.ApproveStockRequest;

public class ApproveStockRequestCommand : IRequest<StockRequestResponseDto>
{
    public Guid StockRequestId { get; set; }
    public Guid DecidedByUserId { get; set; }
 
    public ApproveStockRequestCommand(Guid stockRequestId, Guid decidedByUserId)
    {
        StockRequestId = stockRequestId;
        DecidedByUserId = decidedByUserId;
    }
}
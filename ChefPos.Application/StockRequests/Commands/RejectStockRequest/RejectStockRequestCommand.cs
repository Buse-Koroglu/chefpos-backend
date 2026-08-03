using ChefPos.Application.StockRequests.DTOs;
using MediatR;

namespace ChefPos.Application.StockRequests.Commands.RejectStockRequest;

public class RejectStockRequestCommand : IRequest<StockRequestResponseDto>
{
    public Guid StockRequestId { get; set; }
    public Guid DecidedByUserId { get; set; }
    public string Reason { get; set; } = default!;
 
    public RejectStockRequestCommand(Guid stockRequestId, Guid decidedByUserId, string reason)
    {
        StockRequestId = stockRequestId;
        DecidedByUserId = decidedByUserId;
        Reason = reason;
    }
}

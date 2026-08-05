using ChefPos.Application.StockRequests.DTOs;
using MediatR;

namespace ChefPos.Application.StockRequests.Commands.RejectStockRequest;

public class RejectStockRequestCommand : IRequest<StockRequestResponseDto>
{
    public Guid StockRequestId { get; set; }
    public string Reason { get; set; } = default!;

    public RejectStockRequestCommand(Guid stockRequestId, string reason)
    {
        StockRequestId = stockRequestId;
        Reason = reason;
    }
}

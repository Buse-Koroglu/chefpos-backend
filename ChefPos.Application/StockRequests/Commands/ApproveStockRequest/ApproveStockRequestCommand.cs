using ChefPos.Application.StockRequests.DTOs;
using MediatR;

namespace ChefPos.Application.StockRequests.Commands.ApproveStockRequest;

public class ApproveStockRequestCommand : IRequest<StockRequestResponseDto>
{
    public Guid StockRequestId { get; set; }
    public decimal UnitPrice { get; set; }

    public ApproveStockRequestCommand(Guid stockRequestId, decimal unitPrice)
    {
        StockRequestId = stockRequestId;
        UnitPrice = unitPrice;
    }
}
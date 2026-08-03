namespace ChefPos.Application.StockRequests.DTOs;

public class RejectStockRequestDto
{
    public Guid DecidedByUserId { get; set; }
    public string Reason { get; set; } = default!;
}
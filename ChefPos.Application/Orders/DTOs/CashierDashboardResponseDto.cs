namespace ChefPos.Application.Orders.DTOs;

public class CashierDashboardResponseDto
{
    public int PendingOrdersCount { get; set; }
    public decimal TodayRevenue { get; set; }
    public BestSellingProductDto? BestSellingProduct { get; set; }
}
 
public class BestSellingProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int TotalQuantitySold { get; set; }
}

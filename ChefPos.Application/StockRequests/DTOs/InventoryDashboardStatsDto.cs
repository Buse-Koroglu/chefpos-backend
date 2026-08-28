namespace ChefPos.Application.StockRequests.DTOs;

public class InventoryDashboardStatsDto
{
    public int PendingRequestsCount { get; set; }
    public int PastRequestsCount { get; set; }
    public int TotalStockRequestsCount { get; set; }

    public InventoryDashboardStatsDto(int pendingRequestsCount,int pastRequestsCount, int totalStockRequestsCount){
        PendingRequestsCount = pendingRequestsCount;
        PastRequestsCount = pastRequestsCount;
        TotalStockRequestsCount = totalStockRequestsCount;
    }
}


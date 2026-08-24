
public class DashboardSummaryDto {
    public int TotalStaffCount { get; set; }
    public string? TopSellingProductName { get; set; }
    public List<DashboardDailyRevenueDto> WeeklyRevenue { get; set; } = new();
    public List<LocationOrderCountDto> TodayOrdersByLocation { get; set; } = new();
}

public class DashboardDailyRevenueDto
{
    public DateTime Date { get; set; }
    public decimal Profit { get; set; }
}

public class LocationOrderCountDto
{
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = default!;
    public int OrderCount { get; set; }
}
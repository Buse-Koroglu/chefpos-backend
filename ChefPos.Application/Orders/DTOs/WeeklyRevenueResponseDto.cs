namespace ChefPos.Application.Orders.DTOs;

public class WeeklyRevenueResponseDto
{
    public List<DailyRevenueDto> Days { get; set; } = new();
}

public class DailyRevenueDto
{
    public DateTime Date { get; set; }
    public string DayName { get; set; } = default!;
    public decimal Profit { get; set; }
}

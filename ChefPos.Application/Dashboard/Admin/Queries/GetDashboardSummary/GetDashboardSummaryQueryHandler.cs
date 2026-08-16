using ChefPos.Application.Common.Interfaces;
using MediatR;

namespace ChefPos.Application.Dashboard.Admin.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILocationRepository _locationRepository;

    public GetDashboardSummaryQueryHandler(
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ILocationRepository locationRepository)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _locationRepository = locationRepository;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        // 1. Toplam personel sayısı
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        var totalStaffCount = allUsers.Count;

        // 2. En çok satan ürün (seçili lokasyon için)
        var bestSeller = await _orderRepository.GetBestSellingProductAsync(request.LocationId, cancellationToken);
        string? topSellingProductName = null;
        if (bestSeller.HasValue)
        {
            var product = await _productRepository.GetByIdAsync(bestSeller.Value.ProductId, cancellationToken);
            topSellingProductName = product?.Name;
        }

        var today = DateTime.UtcNow.Date;
        var daysSinceMonday = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var mondayOfThisWeek = today.AddDays(-daysSinceMonday);
        var fridayOfThisWeek = mondayOfThisWeek.AddDays(4);
        var toDateExclusive = fridayOfThisWeek.AddDays(1);

        var dailyRevenueRaw = await _orderRepository.GetDailyRevenueAsync(request.LocationId, mondayOfThisWeek, toDateExclusive, cancellationToken);

        var weeklyRevenue = Enumerable.Range(0, 5)
            .Select(offset =>
            {
                var date = mondayOfThisWeek.AddDays(offset);
                var revenue = dailyRevenueRaw.FirstOrDefault(x => x.Date.Date == date).Revenue;
                return new DashboardDailyRevenueDto { Date = date, Revenue = revenue };
            })
            .ToList();

        var todayOrdersRaw = await _orderRepository.GetTodayPaidOrderCountByLocationAsync(cancellationToken);
        var allLocations = await _locationRepository.GetAllAsync(false,cancellationToken);

        var todayOrdersByLocation = allLocations
            .Select(loc => new LocationOrderCountDto
            {
                LocationId = loc.Id,
                LocationName = loc.Name,
                OrderCount = todayOrdersRaw.FirstOrDefault(x => x.LocationId == loc.Id).OrderCount
            })
            .ToList();

        return new DashboardSummaryDto
        {
            TotalStaffCount = totalStaffCount,
            TopSellingProductName = topSellingProductName,
            WeeklyRevenue = weeklyRevenue,
            TodayOrdersByLocation = todayOrdersByLocation
        };
    }
}
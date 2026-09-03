using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Dashboard.Admin.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetDashboardSummaryQueryHandler(
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ILocationRepository locationRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _locationRepository = locationRepository;
        _currentUserService = currentUserService;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        var isSuperAdmin = actingUser.HasRole(Role.SUPER_ADMIN);
        var locationId = isSuperAdmin
            ? request.LocationId
            : actingUser.LocationIdsForRole(Role.ADMIN).FirstOrDefault();

        // 1. Toplam personel sayısı
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        var totalStaffCount = isSuperAdmin
            ? allUsers.Count
            : allUsers.Count(u => u.Locations.Any(l => l.LocationId == locationId));

        // 2. En çok satan ürün (seçili lokasyon için)
        var bestSeller = await _orderRepository.GetBestSellingProductAsync(locationId, cancellationToken);
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

        var dailyProfitRaw = await _orderRepository.GetDailyProfitAsync(locationId, mondayOfThisWeek, toDateExclusive, cancellationToken);

        var weeklyRevenue = Enumerable.Range(0, 5)
            .Select(offset =>
            {
                var date = mondayOfThisWeek.AddDays(offset);
                var profit = dailyProfitRaw.FirstOrDefault(x => x.Date.Date == date).Profit;
                return new DashboardDailyRevenueDto { Date = date, Profit = profit };
            })
            .ToList();

        var todayOrdersRaw = await _orderRepository.GetTodayPaidOrderCountByLocationAsync(cancellationToken);
        var allLocations = await _locationRepository.GetAllAsync(false,cancellationToken);
        var visibleLocations = isSuperAdmin ? allLocations : allLocations.Where(loc => loc.Id == locationId);

        var todayOrdersByLocation = visibleLocations
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
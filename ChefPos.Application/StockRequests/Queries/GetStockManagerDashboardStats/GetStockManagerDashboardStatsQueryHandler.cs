using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Application.StockRequests.Queries.GetStockManagerDashboardStats;
using ChefPos.Domain.Enums;
using MediatR;

public class GetStockManagerDashboardStatsQueryHandler : IRequestHandler<GetStockManagerDashboardStatsQuery, StockManagerDashboardStatsDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IStockRequestRepository _stockRequestRepository;
    private readonly IUserRepository _userRepository;

    public GetStockManagerDashboardStatsQueryHandler(ICurrentUserService currentUserService,IStockRequestRepository stockRequestRepository, IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _stockRequestRepository = stockRequestRepository;
        _userRepository = userRepository;
    }

    public async Task<StockManagerDashboardStatsDto> Handle( GetStockManagerDashboardStatsQuery request, CancellationToken cancellationToken) {
        var userId = _currentUserService.UserId;

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            throw new NotFoundException($"Kullanıcı bulunamadı: {userId}");
        }

        if (!user.HasRole(Role.STOCK_MANAGER) && !user.HasRole(Role.ADMIN) && !user.HasRole(Role.SUPER_ADMIN))
        {
            throw new ValidationException("Dashboard'a erişim yetkiniz yok.");
        }

        if (!user.HasRole(Role.SUPER_ADMIN) && !user.HasAccessToLocation(request.LocationId))
        {
            throw new ValidationException("Bu yerleşkeye erişim yetkiniz yok.");
        }

        var stats = await _stockRequestRepository.GetStockManagerDashboardStatsAsync(request.LocationId, cancellationToken);

        return new StockManagerDashboardStatsDto( stats.PendingRequestsCount, stats.PastRequestsCount, stats.TotalStockRequestsCount);
    }
}

using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Application.StockRequests.Queries.GetInventoryDashboardStats;
using ChefPos.Domain.Enums;
using MediatR;

public class GetInventoryDashboardStatsQueryHandler : IRequestHandler<GetInventoryDashboardStatsQuery, InventoryDashboardStatsDto> {
    private readonly ICurrentUserService _currentUserService;
    private readonly IStockRequestRepository _stockRequestRepository;
    private readonly IUserRepository _userRepository;

    public GetInventoryDashboardStatsQueryHandler(ICurrentUserService currentUserService, IStockRequestRepository stockRequestRepository, IUserRepository userRepository)
    {
        _currentUserService = currentUserService;
        _stockRequestRepository = stockRequestRepository;
        _userRepository = userRepository;
    }

    public async Task<InventoryDashboardStatsDto> Handle(GetInventoryDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {userId}");

        if (!user.HasRole(Role.INVENTORY_STAFF) && !user.HasRole(Role.ADMIN) && !user.HasRole(Role.SUPER_ADMIN))
        {
            throw new ValidationException("Dashboard'a erişim yetkiniz yok.");
        }

        var locationId = request.LocationId;

        if (locationId.HasValue && !user.HasRole(Role.SUPER_ADMIN))
        {
            var hasLocationAccess = user.HasRoleAtLocation(Role.INVENTORY_STAFF, locationId.Value)
                || user.HasRoleAtLocation(Role.ADMIN, locationId.Value);
            if (!hasLocationAccess)
                throw new ValidationException("Bu yerleşkeye erişim yetkiniz yok.");
        }

        var stats = await _stockRequestRepository.GetInventoryDashboardStatsAsync(userId, locationId, cancellationToken);

        return new InventoryDashboardStatsDto(stats.PendingRequestsCount, stats.PastRequestsCount, stats.TotalStockRequestsCount);
    }
}
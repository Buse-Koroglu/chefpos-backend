using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.Common.Interfaces;

public interface IStockRequestRepository
{
    Task<StockRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<StockRequest>> GetAllByLocationAsync(Guid locationId, StockRequestStatus? status, CancellationToken cancellationToken);
    Task<bool> HasPendingAsync(Guid ingredientId, Guid locationId, CancellationToken cancellationToken);
    Task AddAsync(StockRequest stockRequest, CancellationToken cancellationToken);
    Task<(List<StockRequest> Items, int TotalCount)> GetAllPagedAsync(
        string? searchTerm,
        Guid? locationId,
        StockRequestStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    
    Task<(
        int PendingRequestsCount,
        int PastRequestsCount,
        int TotalStockRequestsCount
        )> GetInventoryDashboardStatsAsync(
        Guid userId,
        Guid? locationId,
        CancellationToken cancellationToken);
    Task SaveAllChangesAsync(CancellationToken cancellationToken);
}
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.Common.Interfaces;

public interface IOrderRepository
{
    Task<Order?>  GetByIdAsync(Guid id,CancellationToken cancellationToken);
    Task<List<Order>> GetAllByLocationAsync(Guid locationId, OrderStatus? status, OrderType? orderType, CancellationToken cancellationToken);
    Task<Order?> GetOpenOrderByTableIdAsync(Guid tableId, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<(List<Order> Items, int TotalCount)> GetAllByLocationPagedAsync(
        Guid locationId,
        OrderStatus? status,
        OrderType? orderType,
        PaymentStatus? paymentStatus,
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    
    Task<int> GetPendingOrdersCountAsync(Guid locationId, CancellationToken cancellationToken);
    Task<decimal> GetTodayRevenueAsync(Guid locationId, CancellationToken cancellationToken);
    Task<(Guid ProductId, int TotalQuantitySold)?> GetBestSellingProductAsync(Guid locationId, CancellationToken cancellationToken);
    Task<List<(DateTime Date, decimal Profit)>> GetDailyProfitAsync(Guid locationId, DateTime fromDate, DateTime toDateExclusive, CancellationToken cancellationToken);

    Task<List<(Guid LocationId, int OrderCount)>> GetTodayPaidOrderCountByLocationAsync(CancellationToken cancellationToken);
    Task SaveAllChangesAsync(CancellationToken cancellationToken);
    
}
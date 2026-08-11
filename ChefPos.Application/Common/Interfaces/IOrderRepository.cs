using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.Common.Interfaces;

public interface IOrderRepository
{
    Task<Order?>  GetByIdAsync(Guid id,CancellationToken cancellationToken);
    Task<List<Order>> GetAllByLocationAsync(Guid locationId, OrderStatus? status, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task<(List<Order> Items, int TotalCount)> GetPagedAsync(
        Guid? locationId,
        OrderStatus? status,
        PaymentStatus? paymentStatus,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    
    Task<int> GetPendingOrdersCountAsync(Guid locationId, CancellationToken cancellationToken);
    Task<decimal> GetTodayRevenueAsync(Guid locationId, CancellationToken cancellationToken);
    Task<(Guid ProductId, int TotalQuantitySold)?> GetBestSellingProductAsync(Guid locationId, CancellationToken cancellationToken);
    Task<List<(DateTime Date, decimal Revenue)>> GetDailyRevenueAsync(Guid locationId, DateTime fromDate, DateTime toDateExclusive, CancellationToken cancellationToken);

    Task SaveAllChangesAsync(CancellationToken cancellationToken);
    
}
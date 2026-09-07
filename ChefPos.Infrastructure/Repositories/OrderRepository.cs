using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using ChefPos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChefPos.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{

    private readonly ApplicationDbContext _context;
    private readonly IBusinessClock _businessClock;
    public OrderRepository(ApplicationDbContext context, IBusinessClock businessClock)
    {
        _context = context;
        _businessClock = businessClock;
    }
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Orders.Include(o => o.Items).Include(o => o.Table).FirstOrDefaultAsync(o => o.Id == id,cancellationToken);
    }

    public async Task<(List<Order> Items, int TotalCount)> GetKitchenOrdersPagedAsync(
        Guid locationId,
        OrderStatus? status,
        IReadOnlyList<OrderType> orderTypes,
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var types = orderTypes.ToArray();

        var query = _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Table)
            .Where(o => o.LocationId == locationId)
            .Where(o => types.Contains(o.OrderType));

        if (status.HasValue)
        {
            query = query.Where(o => o.OrderStatus == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            var hasOrderNumber = int.TryParse(term, out var orderNumber);

            query = query.Where(o =>
                EF.Functions.ILike(o.CustomerName ?? string.Empty, $"%{term}%")
                || (hasOrderNumber && o.OrderNumber == orderNumber));
        }

        query = query.OrderBy(o => o.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<(List<Order> Items, int TotalCount)> GetAllByLocationPagedAsync(Guid locationId, OrderStatus? status, OrderType? orderType, PaymentStatus? paymentStatus, string? searchTerm, Guid? createdByUserId, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        IQueryable<Order> ApplyCommonFilters(IQueryable<Order> source)
        {
            source = source.Where(o => o.LocationId == locationId);

            if (status.HasValue)
            {
                source = source.Where(o => o.OrderStatus == status.Value);
            }

            if (orderType.HasValue)
            {
                source = source.Where(o => o.OrderType == orderType.Value);
            }

            if (paymentStatus.HasValue)
            {
                source = source.Where(o => o.PaymentStatus == paymentStatus.Value);
            }

            if (createdByUserId.HasValue)
            {
                source = source.Where(o => o.CreatedByUserId == createdByUserId.Value);
            }

            if (fromDate.HasValue)
            {
                source = source.Where(o => o.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                source = source.Where(o => o.CreatedAt <= toDate.Value);
            }

            return source;
        }

        var query = ApplyCommonFilters(_context.Orders.Include(o => o.Items).Include(o => o.Table));

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearchTerm = searchTerm.Trim();

            query = query.Where(o => EF.Functions.ILike(o.CustomerName ?? string.Empty, $"%{normalizedSearchTerm}%"));

            if (int.TryParse(normalizedSearchTerm, out var orderNumber))
            {
                query = query.Union(ApplyCommonFilters(_context.Orders.Include(o => o.Items).Include(o => o.Table)).Where(o => o.OrderNumber == orderNumber));
            }
        }

        query = query.OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items, totalCount);
    }
    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public async Task<Order?> GetOpenOrderByTableIdAsync(Guid tableId, CancellationToken cancellationToken) // masaya ait o anki sipariş
    {
        return await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.Table)
            .FirstOrDefaultAsync(o => o.TableId == tableId && o.OrderStatus != OrderStatus.CANCELLED && o.PaymentStatus == PaymentStatus.UNPAID, cancellationToken);
    }
     public async Task<(List<Order> Items, int TotalCount)> GetPagedAsync(Guid? locationId, OrderStatus? status, PaymentStatus? paymentStatus, DateTime? fromDate, DateTime? toDate, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Orders.Include(o => o.Items).Include(o => o.Table).AsQueryable();
 
        if (locationId.HasValue)
        {
            query = query.Where(o => o.LocationId == locationId.Value);
        }
 
        if (status.HasValue)
        {
            query = query.Where(o => o.OrderStatus == status.Value);
        }
 
        if (paymentStatus.HasValue)
        {
            query = query.Where(o => o.PaymentStatus == paymentStatus.Value);
        }
 
        if (fromDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt >= fromDate.Value);
        }
 
        if (toDate.HasValue)
        {
            query = query.Where(o => o.CreatedAt <= toDate.Value);
        }
 
        var totalCount = await query.CountAsync(cancellationToken);
 
        var items = await query.OrderByDescending(o => o.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
 
        return (items, totalCount);
    }
 
    public async Task<int> GetPendingOrdersCountAsync(Guid locationId, CancellationToken cancellationToken)
    {
        return await _context.Orders.CountAsync(o => o.LocationId == locationId && o.OrderStatus == OrderStatus.PENDING, cancellationToken);
    }
 
    public async Task<decimal> GetTodayRevenueAsync(Guid locationId, CancellationToken cancellationToken)
    {
        var todayStart = _businessClock.ToUtc(_businessClock.Today);
        var todayEnd = _businessClock.ToUtc(_businessClock.Today.AddDays(1));
        return await _context.Orders.Where(o => o.LocationId == locationId && o.PaymentStatus == PaymentStatus.PAID && o.PaidAt != null && o.PaidAt >= todayStart && o.PaidAt < todayEnd).SumAsync(o => (decimal?)o.TotalPrice, cancellationToken) ?? 0m;
    }
 
    public async Task<(Guid ProductId, int TotalQuantitySold)?> GetBestSellingProductAsync(Guid locationId, CancellationToken cancellationToken)
    {
        var result = await _context.Orders
            .Where(o => o.LocationId == locationId && o.OrderStatus == OrderStatus.COMPLETED)
            .SelectMany(o => o.Items)
            .Where(i => i.ProductId != null)
            .GroupBy(i => i.ProductId!.Value)
            .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(i => i.Quantity) })
            .OrderByDescending(x => x.TotalQuantity)
            .FirstOrDefaultAsync(cancellationToken);
 
        return result is null ? null : (result.ProductId, result.TotalQuantity);
    }
    
    public async Task<List<(DateTime Date, decimal Profit)>> GetDailyProfitAsync(
        Guid locationId, DateTime fromDate, DateTime toDateExclusive, CancellationToken cancellationToken)
    {
        var orders = await _context.Orders
            .Where(o => o.LocationId == locationId
                     && o.OrderStatus == OrderStatus.COMPLETED
                     && o.CompletedAt != null
                     && o.CompletedAt >= fromDate
                     && o.CompletedAt < toDateExclusive)
            .Select(o => new
            {
                o.Id,
                CompletedAt = o.CompletedAt!.Value,
                Revenue = o.TotalPrice
            })
            .ToListAsync(cancellationToken);

        if (orders.Count == 0)
        {
            return new List<(DateTime, decimal)>();
        }

        var orderIds = orders.Select(o => o.Id).ToList();

        var costByOrderId = await (
            from consumption in _context.StockMovementLotConsumptions
            join movement in _context.StockMovements
                on consumption.StockMovementId equals movement.Id
            where movement.Type == StockMovementType.ORDER_SALE
               && movement.RelatedOrderId != null
               && orderIds.Contains(movement.RelatedOrderId.Value)
            group consumption.QuantityConsumed * consumption.UnitPriceAtConsumption
                by movement.RelatedOrderId!.Value
            into grouped
            select new { OrderId = grouped.Key, Cost = grouped.Sum() })
            .ToDictionaryAsync(x => x.OrderId, x => x.Cost, cancellationToken);

        return orders
            .GroupBy(o => _businessClock.GetBusinessDate(o.CompletedAt))
            .Select(g => (
                Date: g.Key,
                Profit: g.Sum(o => o.Revenue - (costByOrderId.TryGetValue(o.Id, out var cost) ? cost : 0m))
            ))
            .OrderBy(x => x.Date)
            .ToList();
    }
    
    public async Task<List<(Guid LocationId, int OrderCount)>> GetTodayPaidOrderCountByLocationAsync(CancellationToken cancellationToken)
    {
        var todayStart = _businessClock.ToUtc(_businessClock.Today);
        var todayEnd = _businessClock.ToUtc(_businessClock.Today.AddDays(1));

        var result = await _context.Orders
            .Where(o => o.PaymentStatus == PaymentStatus.PAID && o.PaidAt != null && o.PaidAt >= todayStart && o.PaidAt < todayEnd)
            .GroupBy(o => o.LocationId)
            .Select(g => new { LocationId = g.Key, OrderCount = g.Count() })
            .ToListAsync(cancellationToken);

        return result.Select(x => (x.LocationId, x.OrderCount)).ToList();
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
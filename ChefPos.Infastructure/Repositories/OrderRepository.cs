using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using ChefPos.Infastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChefPos.Infastructure.Repositories;

public class OrderRepository : IOrderRepository
{

    private readonly ApplicationDbContext _context;
    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Orders.Include(o => o.Items).Include(o => o.Table).FirstOrDefaultAsync(o => o.Id == id,cancellationToken);
    }

    public async Task<List<Order>> GetAllByLocationAsync(Guid locationId, OrderStatus? status, OrderType? orderType, CancellationToken cancellationToken)
    {
        var query = _context.Orders.Include(o => o.Items).Include(o => o.Table).Where(o => o.LocationId == locationId);

        if (status.HasValue)
            query = query.Where(o => o.OrderStatus == status.Value);

        if (orderType.HasValue)
            query = query.Where(o => o.OrderType == orderType.Value);

        return await query.ToListAsync(cancellationToken);
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
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);
        return await _context.Orders.Where(o => o.LocationId == locationId && o.OrderStatus == OrderStatus.COMPLETED && o.CompletedAt != null && o.CompletedAt >= todayStart && o.CompletedAt < todayEnd).SumAsync(o => (decimal?)o.TotalPrice, cancellationToken) ?? 0m;
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
            .Include(o => o.Items)
            .Where(o => o.LocationId == locationId && o.OrderStatus == OrderStatus.COMPLETED && o.CompletedAt != null && o.CompletedAt >= fromDate && o.CompletedAt < toDateExclusive)
            .ToListAsync(cancellationToken);

        var productIds = orders
            .SelectMany(o => o.Items)
            .Where(i => i.ProductId.HasValue)
            .Select(i => i.ProductId!.Value)
            .Distinct()
            .ToList();

        var products = await _context.Products
            .Include(p => p.ProductLocations).ThenInclude(pl => pl.ProductItems).ThenInclude(pi => pi.Ingredient).ThenInclude(i => i.Lots)
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(cancellationToken);

        var ingredientCostByProductId = products.ToDictionary(
            p => p.Id,
            p => p.ProductLocations.Where(pl => pl.LocationId == locationId).SelectMany(pl => pl.ProductItems).Sum(pi => pi.QuantityPerServing * pi.Ingredient.WeightedAverageUnitPrice)); // weighted price üzerinden her bir ürün üzerinden kar hesaplaması

        return orders
            .GroupBy(o => o.CompletedAt!.Value.Date)
            .Select(g => ( Date: g.Key,
                Profit: g.SelectMany(o => o.Items).Sum(i =>
                {
                    var ingredientCost = i.ProductId.HasValue && ingredientCostByProductId.TryGetValue(i.ProductId.Value, out var cost)
                        ? cost
                        : 0m;
                    return (i.Price - ingredientCost) * i.Quantity;
                })
            )).ToList();
    }
    
    public async Task<List<(Guid LocationId, int OrderCount)>> GetTodayPaidOrderCountByLocationAsync(CancellationToken cancellationToken)
    {
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);

        var result = await _context.Orders
            .Where(o => o.PaymentStatus == PaymentStatus.PAID && o.CreatedAt >= todayStart && o.CreatedAt < todayEnd)
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
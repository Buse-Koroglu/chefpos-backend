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
        return await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id,cancellationToken);
    }
    
    public async Task<List<Order>> GetAllByLocationAsync(Guid locationId, OrderStatus? status, CancellationToken cancellationToken)
    {
        var query = _context.Orders
            .Include(o => o.Items)
            .Where(o => o.LocationId == locationId);
 
        if (status.HasValue)
            query = query.Where(o => o.OrderStatus == status.Value);
 
        return await query
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Order order, CancellationToken cancellationToken)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
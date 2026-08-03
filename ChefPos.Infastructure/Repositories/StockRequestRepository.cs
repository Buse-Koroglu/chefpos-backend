using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using ChefPos.Infastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class StockRequestRepository : IStockRequestRepository
{
    private readonly ApplicationDbContext _context;
    public StockRequestRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<StockRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.StockRequests
            .Include(sr => sr.Ingredient)
            .FirstOrDefaultAsync(sr => sr.Id == id, cancellationToken);
    }

    public async Task<List<StockRequest>> GetAllByLocationAsync(Guid locationId, StockRequestStatus? status, CancellationToken cancellationToken)
    {
        var query = _context.StockRequests
            .Include(sr => sr.Ingredient)
            .Where(sr => sr.LocationId == locationId);

        if (status.HasValue)
        {
            query = query.Where(sr => sr.Status == status.Value);
        }

        return await query.OrderByDescending(sr => sr.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPendingAsync(Guid ingredientId, Guid locationId, CancellationToken cancellationToken)
    {
        return await _context.StockRequests.AnyAsync(sr =>
            sr.IngredientId == ingredientId &&
            sr.LocationId == locationId &&
            sr.Status == StockRequestStatus.PENDING, cancellationToken);
    }

    public async Task AddAsync(StockRequest stockRequest, CancellationToken cancellationToken)
    {
        await _context.StockRequests.AddAsync(stockRequest, cancellationToken);
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
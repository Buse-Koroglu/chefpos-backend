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

    public async Task<(List<StockRequest> Items, int TotalCount)> GetAllPagedAsync(
        string? searchTerm,
        Guid? locationId,
        StockRequestStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.StockRequests
            .AsNoTracking()
            .Include(sr => sr.Ingredient)
            .Include(sr => sr.Location)
            .Include(sr => sr.RequestedByUser)
            .Include(sr => sr.DecidedByUser) 
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(sr => EF.Functions.ILike(sr.Ingredient.Name, $"%{searchTerm}%"));
        }

        if (locationId.HasValue && locationId.Value != Guid.Empty)
        {
            query = query.Where(sr => sr.LocationId == locationId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(sr => sr.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(sr => sr.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<(
        int PendingRequestsCount,
        int PastRequestsCount,
        int TotalStockRequestsCount
        )> GetInventoryDashboardStatsAsync(
        Guid userId,
        Guid? locationId,
        CancellationToken cancellationToken)
    {
        var query = _context.StockRequests
            .AsNoTracking()
            .Where(request =>
                request.RequestedByUserId == userId);

        if (locationId.HasValue)
        {
            query = query.Where(request =>
                request.LocationId == locationId.Value);
        }

        var pendingCount = await query
            .CountAsync(
                request =>
                    request.Status == StockRequestStatus.PENDING,
                cancellationToken);

        var pastCount = await query
            .CountAsync(
                request =>
                    request.Status == StockRequestStatus.APPROVED ||
                    request.Status == StockRequestStatus.REJECTED,
                cancellationToken);

        var totalCount = await query
            .CountAsync(cancellationToken);

        return (
            pendingCount,
            pastCount,
            totalCount
        );
    }
}
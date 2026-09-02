
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using ChefPos.Infrastructure.Persistence;
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
        return await _context.StockRequests.Include(sr => sr.Ingredient).FirstOrDefaultAsync(sr => sr.Id == id, cancellationToken);
    }

    public async Task<List<StockRequest>> GetAllByLocationAsync(Guid locationId, StockRequestStatus? status, CancellationToken cancellationToken)
    {
        var query = _context.StockRequests.Include(sr => sr.Ingredient).Where(sr => sr.LocationId == locationId);

        if (status.HasValue)
        {
            query = query.Where(sr => sr.Status == status.Value);
        }

        return await query.OrderByDescending(sr => sr.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPendingAsync(Guid ingredientId, Guid locationId, CancellationToken cancellationToken)
    {
        return await _context.StockRequests.AnyAsync(sr => sr.IngredientId == ingredientId && sr.LocationId == locationId && sr.Status == StockRequestStatus.PENDING, cancellationToken);
    }

    public async Task AddAsync(StockRequest stockRequest, CancellationToken cancellationToken)
    {
        await _context.StockRequests.AddAsync(stockRequest, cancellationToken);
    }

    public async Task<(List<StockRequest> Items, int TotalCount)> GetAllPagedAsync(string? searchTerm, Guid? locationId, StockRequestStatus? status, Guid? requestedByUserId, bool onlyHistory, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = BuildFilteredQuery(searchTerm, locationId, status, requestedByUserId, onlyHistory, startDate, endDate);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(sr => sr.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<List<StockRequest>> GetAllForExportAsync(
        string? searchTerm,
        Guid? locationId,
        StockRequestStatus? status,
        Guid? requestedByUserId,
        bool onlyHistory,
        DateTime? startDate,
        DateTime? endDate,
        int maxRows,
        CancellationToken cancellationToken)
    {
        var query = BuildFilteredQuery(searchTerm, locationId, status, requestedByUserId, onlyHistory, startDate, endDate);

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount > maxRows) throw new ValidationException(ExportLimits.ExceededMessage);

        return await query.OrderByDescending(sr => sr.CreatedAt).ToListAsync(cancellationToken);
    }

    private IQueryable<StockRequest> BuildFilteredQuery(string? searchTerm, Guid? locationId, StockRequestStatus? status, Guid? requestedByUserId, bool onlyHistory, DateTime? startDate, DateTime? endDate)
    {
        var query = _context.StockRequests .AsNoTracking()
            .Include(sr => sr.Ingredient).ThenInclude(i => i.Lots)
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

        if (onlyHistory)
        {
            query = query.Where(sr => sr.Status != StockRequestStatus.PENDING);
        }

        if (requestedByUserId.HasValue && requestedByUserId.Value != Guid.Empty)
        {
            query = query.Where(sr => sr.RequestedByUserId == requestedByUserId.Value);
        }

        if (startDate.HasValue)
        {
            var startUtc = DateTime.SpecifyKind(startDate.Value.Date, DateTimeKind.Utc);
            query = query.Where(sr => sr.CreatedAt >= startUtc);
        }

        if (endDate.HasValue)
        {
            var endUtc = DateTime.SpecifyKind(endDate.Value.Date.AddDays(1), DateTimeKind.Utc);
            query = query.Where(sr => sr.CreatedAt < endUtc);
        }

        return query;
    }
    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    // depo görevlisi dashboard için kendi taleplerini içeren bir dash
    public async Task<(int PendingRequestsCount, int PastRequestsCount, int TotalStockRequestsCount)> GetInventoryDashboardStatsAsync( Guid userId, Guid? locationId, CancellationToken cancellationToken)
    {
        var query = _context.StockRequests.AsNoTracking().Where(request => request.RequestedByUserId == userId);

        if (locationId.HasValue)
        {
            query = query.Where(request => request.LocationId == locationId.Value);
        }

        var pendingCount = await query.CountAsync(request => request.Status == StockRequestStatus.PENDING,cancellationToken);

        var pastCount = await query.CountAsync(request => request.Status == StockRequestStatus.APPROVED || request.Status == StockRequestStatus.REJECTED, cancellationToken);

        var totalCount = await query.CountAsync(cancellationToken);

        return (pendingCount, pastCount, totalCount);
    }
    // stok yöneticisi dashboard için lokasyon bazlı tüm talepleri içeren bir dash
    public async Task<(int PendingRequestsCount, int PastRequestsCount,int TotalStockRequestsCount)> GetStockManagerDashboardStatsAsync(Guid locationId, CancellationToken cancellationToken)
    {
        var query = _context.StockRequests.AsNoTracking().Where(request => request.LocationId == locationId);

        var pendingCount = await query.CountAsync(request => request.Status == StockRequestStatus.PENDING,cancellationToken);

        var pastCount = await query.CountAsync(request => request.Status == StockRequestStatus.APPROVED || request.Status == StockRequestStatus.REJECTED, cancellationToken);

        var totalCount = await query.CountAsync(cancellationToken);

        return (pendingCount, pastCount, totalCount);
    }
}
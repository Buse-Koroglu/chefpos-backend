using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChefPos.Infrastructure.Repositories;

public class TableRepository : ITableRepository
{
    private readonly ApplicationDbContext _context;

    public TableRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Table?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Tables.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Table>> GetAllByLocationAsync(Guid locationId, bool includeInactive, CancellationToken cancellationToken)
    {
        var query = _context.Tables.Where(t => t.LocationId == locationId);

        if (!includeInactive)
        {
            query = query.Where(t => t.IsActive);
        }

        return await query.OrderBy(t => t.TableNumber).ToListAsync(cancellationToken);
    }

    public async Task<(List<Table> Items, int TotalCount)> GetAllPagedAsync(string? searchTerm, Guid? locationId, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Tables.AsQueryable();

        if (locationId.HasValue)
            query = query.Where(t => t.LocationId == locationId.Value);

        if (isActive.HasValue)
            query = query.Where(t => t.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm) && int.TryParse(searchTerm, out var searchNumber))
            query = query.Where(t => t.TableNumber == searchNumber);

        query = query.OrderBy(t => t.TableNumber);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<List<Table>> GetAllForExportAsync(string? searchTerm, Guid? locationId, bool? isActive, int maxRows,CancellationToken cancellationToken) {
        var query = _context.Tables.AsNoTracking().Include(t => t.Location).AsQueryable(); // çektiğimiz entity'nin change tracker tarafından takip edilmeisni önler ve performnas artımı sağlar.

        if (locationId.HasValue)
            query = query.Where(t => t.LocationId == locationId.Value);

        if (isActive.HasValue)
            query = query.Where(t => t.IsActive == isActive.Value);

        if (!string.IsNullOrWhiteSpace(searchTerm) && int.TryParse(searchTerm, out var searchNumber))
            query = query.Where(t => t.TableNumber == searchNumber);

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount > maxRows)
            throw new ValidationException(ExportLimits.ExceededMessage);

        return await query.OrderBy(t => t.TableNumber).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNumberAsync(Guid locationId, int tableNumber, Guid? excludeTableId, CancellationToken cancellationToken)
    {
        return await _context.Tables.AnyAsync(t => t.LocationId == locationId && t.TableNumber == tableNumber && t.Id != excludeTableId, cancellationToken);
    }

    public async Task AddAsync(Table table, CancellationToken cancellationToken)
    {
        await _context.Tables.AddAsync(table, cancellationToken);
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}

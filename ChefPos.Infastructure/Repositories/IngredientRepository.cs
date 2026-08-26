using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Infastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class IngredientRepository : IIngredientRepository
{
    private readonly ApplicationDbContext _context;
    public IngredientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Ingredients
            .Include(i => i.Lots)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<List<Ingredient>> GetAllByLocationAsync(Guid locationId, bool includeInactive, CancellationToken cancellationToken)
    {
        var query = _context.Ingredients
            .Include(i => i.Lots)
            .Where(i => i.LocationId == locationId);

        if (!includeInactive)
        {
            query = query.Where(i => i.IsActive);
        }

        return await query.OrderBy(i => i.Name).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken)
    {
        await _context.Ingredients.AddAsync(ingredient, cancellationToken);
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<List<Ingredient>> GetLowStockAsync(Guid locationId, CancellationToken cancellationToken)
    {
        return await _context.Ingredients
            .Include(i => i.Lots)
            .Where(i => i.LocationId == locationId && i.IsActive && i.CurrentStock < i.MinStockThreshold)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Ingredient> Items, int TotalCount)> GetAllPagedAsync(
        string? searchTerm,
        Guid? locationId,
        bool? isActive,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Ingredients
            .Include(i => i.Location)
            .Include(i => i.Lots)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(i => EF.Functions.ILike(i.Name, $"%{searchTerm}%"));

        if (locationId.HasValue)
            query = query.Where(i => i.LocationId == locationId.Value);

        if (isActive.HasValue)
            query = query.Where(i => i.IsActive == isActive.Value);

        query = query.OrderBy(i => i.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<List<Ingredient>> GetAllForExportAsync(
        string? searchTerm,
        Guid? locationId,
        bool? isActive,
        int maxRows,
        CancellationToken cancellationToken)
    {
        var query = _context.Ingredients
            .AsNoTracking()
            .Include(i => i.Location)
            .Include(i => i.Lots)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(i => EF.Functions.ILike(i.Name, $"%{searchTerm}%"));

        if (locationId.HasValue)
            query = query.Where(i => i.LocationId == locationId.Value);

        if (isActive.HasValue)
            query = query.Where(i => i.IsActive == isActive.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount > maxRows)
            throw new ValidationException(ExportLimits.ExceededMessage);

        return await query.OrderBy(i => i.Name).ToListAsync(cancellationToken);
    }
}
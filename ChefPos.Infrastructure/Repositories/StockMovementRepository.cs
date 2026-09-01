namespace ChefPos.Infastructure.Repositories;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using ChefPos.Infastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly ApplicationDbContext _context;

    public StockMovementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(StockMovement movement, CancellationToken cancellationToken)
    {
        await _context.StockMovements.AddAsync(movement, cancellationToken);
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(List<StockMovement> Items, int TotalCount)> GetAllPagedAsync(Guid? ingredientId, Guid? locationId, StockMovementType? type, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.StockMovements
            .Include(m => m.Ingredient)
            .Include(m => m.PerformedByUser)
            .Include(m => m.LotConsumptions)
            .AsQueryable();

        if (ingredientId.HasValue)
            query = query.Where(m => m.IngredientId == ingredientId.Value);

        if (locationId.HasValue)
            query = query.Where(m => m.LocationId == locationId.Value);

        if (type.HasValue)
            query = query.Where(m => m.Type == type.Value);

        query = query.OrderByDescending(m => m.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
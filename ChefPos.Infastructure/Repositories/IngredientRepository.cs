using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Infastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChefPos.Infastructure.Repositories;

public class IngredientRepository : IIngredientRepository
{
    private readonly ApplicationDbContext _context;
    public IngredientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Ingredients.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<List<Ingredient>> GetAllByLocationAsync(Guid locationId, bool includeInactive, CancellationToken cancellationToken)
    {
        var query = _context.Ingredients.Where(i => i.LocationId == locationId);

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
            .Where(i => i.LocationId == locationId && i.IsActive && i.CurrentStock < i.MinStockThreshold)
            .OrderBy(i => i.Name)
            .ToListAsync(cancellationToken);
    }
}
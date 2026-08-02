namespace ChefPos.Application.Common.Interfaces;

using ChefPos.Domain.Entities;

public interface IIngredientRepository
{
    Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Ingredient>> GetAllByLocationAsync(Guid locationId, bool includeInactive, CancellationToken cancellationToken);
    Task AddAsync(Ingredient ingredient, CancellationToken cancellationToken);
    Task SaveAllChangesAsync(CancellationToken cancellationToken);
    Task<List<Ingredient>> GetLowStockAsync(Guid locationId, CancellationToken cancellationToken);
}
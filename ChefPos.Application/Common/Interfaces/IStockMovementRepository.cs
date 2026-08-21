using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

public interface IStockMovementRepository
{
    Task AddAsync(StockMovement movement, CancellationToken cancellationToken);
    Task SaveAllChangesAsync(CancellationToken cancellationToken);
 
    Task<(List<StockMovement> Items, int TotalCount)> GetAllPagedAsync(
        Guid? ingredientId,
        Guid? locationId,
        StockMovementType? type,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
}
using ChefPos.Domain.Entities;

namespace ChefPos.Application.Common.Interfaces;

public interface IProductRepository
{
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Product>> GetAllByLocationAsync(Guid locationId, Guid? categoryId, bool includeInactive, CancellationToken cancellationToken);
        Task AddAsync(Product product, CancellationToken cancellationToken);
        Task SaveAllChangesAsync(CancellationToken cancellationToken);
    
}
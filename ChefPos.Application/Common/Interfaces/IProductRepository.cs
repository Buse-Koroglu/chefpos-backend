using ChefPos.Domain.Entities;

namespace ChefPos.Application.Common.Interfaces;

public interface IProductRepository
{
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Product>> GetAllByLocationAsync(Guid locationId, Guid? categoryId, bool includeInactive, bool includeUncategorized, CancellationToken cancellationToken);

        Task<(List<Product> Items, int TotalCount)> GetAllPagedAsync(
            string? searchTerm,
            Guid? locationId,
            Guid? categoryId,
            bool? isActive,
            int pageNumber,
            int pageSize,
            bool includeUncategorized,
            CancellationToken cancellationToken);

        Task<List<Product>> GetAllForExportAsync(
            string? searchTerm,
            Guid? locationId,
            Guid? categoryId,
            bool? isActive,
            bool includeUncategorized,
            int maxRows,
            CancellationToken cancellationToken);

        Task AddAsync(Product product, CancellationToken cancellationToken);
        Task SaveAllChangesAsync(CancellationToken cancellationToken);

}
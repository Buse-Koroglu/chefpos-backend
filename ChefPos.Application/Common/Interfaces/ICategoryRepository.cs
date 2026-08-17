using ChefPos.Domain.Entities;

namespace ChefPos.Application.Common.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Category>> GetAllByLocationAsync(Guid locationId, bool includeInactive, CancellationToken cancellationToken);
    Task AddAsync(Category category, CancellationToken cancellationToken);
    Task SaveAllChangesAsync(CancellationToken cancellationToken);
    
    Task<(List<Category> Items, int TotalCount)> GetAllPagedAsync(
        string? searchTerm,
        Guid? locationId,
        bool? isActive,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken);
    Task RemoveAsync(Guid id, CancellationToken cancellationToken);
}
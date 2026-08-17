using ChefPos.Domain.Entities;

namespace ChefPos.Application.Common.Interfaces;

public interface ILocationRepository
{
        Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Location>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken);
        
        Task<(List<Location> Items, int TotalCount)> GetAllPagedAsync(
                string? searchTerm,
                bool? isActive,
                int pageNumber,
                int pageSize,
                CancellationToken cancellationToken);
        Task AddAsync(Location location, CancellationToken cancellationToken);
        Task SaveAllChangesAsync(CancellationToken cancellationToken);
}
using ChefPos.Domain.Entities;

namespace ChefPos.Application.Common.Interfaces;

public interface IMenuRepository
{
    Task<Menu?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Menu>> GetAllByLocationAsync(Guid locationId, bool includeInactive, CancellationToken cancellationToken);
    Task AddAsync(Menu menu, CancellationToken cancellationToken);
    Task SaveAllChangesAsync(CancellationToken cancellationToken);
}
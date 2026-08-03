using ChefPos.Domain.Entities;

namespace ChefPos.Application.Common.Interfaces;

public interface IUserRepository
{
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User?> GetByPersonalIdAsync(string personalId, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<User?> GetStockManagerByLocationAsync(Guid locationId, CancellationToken cancellationToken);
        Task SaveAllChangesAsync(CancellationToken cancellationToken);
}
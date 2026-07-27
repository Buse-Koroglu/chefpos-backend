using ChefPos.Domain.Entities;

namespace ChefPos.Application.Common.Interfaces;

public interface IOrderRepository
{
    Task<Order?>  GetByIdAsync(Guid id,CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task SaveAllChangesAsync(CancellationToken cancellationToken);
}
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.Common.Interfaces;

public interface IOrderRepository
{
    Task<Order?>  GetByIdAsync(Guid id,CancellationToken cancellationToken);
    Task<List<Order>> GetAllByLocationAsync(Guid locationId, OrderStatus? status, CancellationToken cancellationToken);
    Task AddAsync(Order order, CancellationToken cancellationToken);
    Task SaveAllChangesAsync(CancellationToken cancellationToken);
}
using ChefPos.Domain.Entities;

namespace ChefPos.Application.Common.Interfaces;

public interface ITableRepository
{
    Task<Table?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Table>> GetAllByLocationAsync(Guid locationId, bool includeInactive, CancellationToken cancellationToken);

    Task<(List<Table> Items, int TotalCount)> GetAllPagedAsync(string? searchTerm, Guid? locationId, bool? isActive,int pageNumber, int pageSize,CancellationToken cancellationToken);

    Task<List<Table>> GetAllForExportAsync(string? searchTerm,Guid? locationId, bool? isActive, int maxRows,CancellationToken cancellationToken);

    Task<bool> ExistsByNumberAsync(Guid locationId, int tableNumber, Guid? excludeTableId, CancellationToken cancellationToken);
    Task AddAsync(Table table, CancellationToken cancellationToken);
    Task SaveAllChangesAsync(CancellationToken cancellationToken);
}

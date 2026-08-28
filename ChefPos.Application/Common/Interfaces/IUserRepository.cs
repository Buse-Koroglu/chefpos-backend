using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.Common.Interfaces;

public interface IUserRepository
{
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User?> GetByPersonalIdAsync(string personalId, CancellationToken cancellationToken);
        Task AddAsync(User user, CancellationToken cancellationToken);
        Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
        Task<User?> GetStockManagerByLocationAsync(Guid locationId, CancellationToken cancellationToken);
        Task<User?> GetAdminByLocationAsync(Guid locationId, CancellationToken cancellationToken);
        Task<User?> GetSuperAdminAsync(CancellationToken cancellationToken);
        Task<(List<User> Users, int TotalCount)> GetAllPagedAsync(string? searchTerm,Role? role,bool? isActive,Guid? locationId,int pageNumber,int pageSize,CancellationToken cancellationToken);
        Task<List<User>> GetAllForExportAsync(string? searchTerm,Role? role,bool? isActive,Guid? locationId,int maxRows, CancellationToken cancellationToken);
        Task<List<(Guid LocationId, int EmployeeCount)>> GetEmployeeCountsByLocationAsync(CancellationToken cancellationToken);
        Task SaveAllChangesAsync(CancellationToken cancellationToken);
}
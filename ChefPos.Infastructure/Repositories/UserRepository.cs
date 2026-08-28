using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using ChefPos.Infastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChefPos.Infastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Users.Include(u => u.Locations).Include(u=>u.UserRoles).FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByPersonalIdAsync(string personalId, CancellationToken cancellationToken)
    {
        return await _context.Users.Include(u => u.Locations).Include(u=>u.UserRoles).FirstOrDefaultAsync(u => u.PersonalId == personalId, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }
    
    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.Locations)
            .Include(u => u.UserRoles)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<User?> GetStockManagerByLocationAsync(Guid locationId, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.Locations)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.IsActive && u.UserRoles.Any(r => r.Role == Role.STOCK_MANAGER) && u.Locations.Any(l => l.LocationId == locationId), cancellationToken);
    }
    
    public async Task<User?> GetAdminByLocationAsync(Guid locationId, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.Locations)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.UserRoles.Any(r => r.Role == Role.ADMIN) && u.Locations.Any(l => l.LocationId == locationId), cancellationToken);
    }

    public async Task<User?> GetSuperAdminAsync(CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.Locations)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.UserRoles.Any(r => r.Role == Role.SUPER_ADMIN), cancellationToken);
    }

    public async Task<(List<User> Users, int TotalCount)> GetAllPagedAsync(string? searchTerm, Role? role, bool? isActive, Guid? locationId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Users
            .Include(u => u.UserRoles)
            .Include(u => u.Locations).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                EF.Functions.ILike(u.FirstName, $"%{searchTerm}%") ||
                EF.Functions.ILike(u.LastName, $"%{searchTerm}%") ||
                EF.Functions.ILike(u.FirstName + " " + u.LastName, $"%{searchTerm}%") ||
                EF.Functions.ILike(u.PersonalId, $"%{searchTerm}%"));
        }

        if (role.HasValue)
            query = query.Where(u => u.UserRoles.Any(ur => ur.Role == role.Value));

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        if (locationId.HasValue)
            query = query.Where(u => u.Locations.Any(l => l.LocationId == locationId.Value));

        query = query.OrderByDescending(u => u.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (users, totalCount);
    }
    public async Task<List<User>> GetAllForExportAsync(string? searchTerm,Role? role,bool? isActive,Guid? locationId,int maxRows,CancellationToken cancellationToken)
    {
        var query = _context.Users
            .AsNoTracking() // çektiğimiz entity'nin change tracker tarafından takip edilmeisni önler ve performnas artımı sağlar.
            .Include(u => u.UserRoles)
            .Include(u => u.Locations).ThenInclude(l => l.Location)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(u =>
                EF.Functions.ILike(u.FirstName, $"%{searchTerm}%") ||
                EF.Functions.ILike(u.LastName, $"%{searchTerm}%") ||
                EF.Functions.ILike(u.FirstName + " " + u.LastName, $"%{searchTerm}%") ||
                EF.Functions.ILike(u.PersonalId, $"%{searchTerm}%"));
        }

        if (role.HasValue)
            query = query.Where(u => u.UserRoles.Any(ur => ur.Role == role.Value));

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        if (locationId.HasValue)
            query = query.Where(u => u.Locations.Any(l => l.LocationId == locationId.Value));

        var totalCount = await query.CountAsync(cancellationToken);
        
        if (totalCount > maxRows)
            throw new ValidationException(ExportLimits.ExceededMessage);

        return await query.OrderByDescending(u => u.CreatedAt).ToListAsync(cancellationToken);
    }

    public async Task<List<(Guid LocationId, int EmployeeCount)>> GetEmployeeCountsByLocationAsync(CancellationToken cancellationToken)
    {
        var result = await _context.UserLocations.GroupBy(ul => ul.LocationId)
            .Select(g => new { LocationId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        return result.Select(x => (x.LocationId, x.Count)).ToList();
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
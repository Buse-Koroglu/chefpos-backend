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
    
    public async Task<User?> GetStockManagerByLocationAsync(Guid locationId, CancellationToken cancellationToken)
    {
        return await _context.Users
            .Include(u => u.Locations)
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u =>
                u.UserRoles.Any(r => r.Role == Role.STOCK_MANAGER) &&
                u.Locations.Any(l => l.LocationId == locationId), cancellationToken);
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
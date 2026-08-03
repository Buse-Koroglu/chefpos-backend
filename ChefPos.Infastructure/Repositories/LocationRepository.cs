using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Infastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class LocationRepository : ILocationRepository
{
    private readonly ApplicationDbContext _context;
    public LocationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Location?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Locations.FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
    }

    public async Task<List<Location>> GetAllAsync(bool includeInactive, CancellationToken cancellationToken)
    {
        var query = _context.Locations.AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(l => l.IsActive);
        }

        return await query.OrderBy(l => l.Name).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Location location, CancellationToken cancellationToken)
    {
        await _context.Locations.AddAsync(location, cancellationToken);
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
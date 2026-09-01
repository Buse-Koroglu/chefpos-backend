using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Infastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChefPos.Infastructure.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly ApplicationDbContext _context;
    public MenuRepository(ApplicationDbContext context) => _context = context;

    public async Task<Menu?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Menus.Include(m => m.MenuProducts).ThenInclude(mp => mp.Product).FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<List<Menu>> GetAllByLocationAsync(Guid locationId, bool includeInactive, CancellationToken cancellationToken)
    {
        var query = _context.Menus.Include(m => m.MenuProducts).ThenInclude(mp => mp.Product).Where(m => m.LocationId == locationId);

        if (!includeInactive)
        {
            query = query.Where(m => m.IsActive);
        }

        return await query.OrderBy(m => m.Name).ToListAsync(cancellationToken);
    }

    public async Task<List<Menu>> GetAllForExportAsync(Guid locationId, bool includeInactive, int maxRows, CancellationToken cancellationToken)
    {
        var query = _context.Menus.AsNoTracking()
            .Include(m => m.Location)
            .Include(m => m.MenuProducts).ThenInclude(mp => mp.Product)
            .Where(m => m.LocationId == locationId);

        if (!includeInactive)
            query = query.Where(m => m.IsActive);

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount > maxRows)
            throw new ValidationException(ExportLimits.ExceededMessage);

        return await query.OrderBy(m => m.Name).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Menu menu, CancellationToken cancellationToken)
    {
        await _context.Menus.AddAsync(menu, cancellationToken);
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
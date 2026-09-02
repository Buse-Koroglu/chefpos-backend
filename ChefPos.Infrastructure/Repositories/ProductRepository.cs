using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChefPos.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;
    
    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Products.Include(p => p.ProductLocations).ThenInclude(pl => pl.Location)
            .Include(p => p.ProductLocations).ThenInclude(pl => pl.ProductItems).ThenInclude(pi => pi.Ingredient).ThenInclude(i => i.Lots)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Product>> GetAllByLocationAsync(Guid locationId, Guid? categoryId, bool includeInactive, bool includeUncategorized, CancellationToken cancellationToken)
    {
        var query = _context.Products.Include(p => p.ProductLocations).ThenInclude(pl => pl.Location)
            .Include(p => p.ProductLocations).ThenInclude(pl => pl.ProductItems).ThenInclude(pi => pi.Ingredient)
            .Where(p => p.ProductLocations.Any(pl => pl.LocationId == locationId));

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!includeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        query = query.Where(p => includeUncategorized || p.CategoryId != null);

        return await query.OrderBy(p => p.Name).ToListAsync(cancellationToken);
    }

    public async Task<(List<Product> Items, int TotalCount)> GetAllPagedAsync(string? searchTerm,Guid? locationId, Guid? categoryId, bool? isActive, int pageNumber, int pageSize, bool includeUncategorized, CancellationToken cancellationToken)
    {
        var query = _context.Products.Include(p => p.Category)
            .Include(p => p.ProductLocations).ThenInclude(pl => pl.Location)
            .Include(p => p.ProductLocations).ThenInclude(pl => pl.ProductItems).ThenInclude(pi => pi.Ingredient)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{searchTerm}%"));

        if (locationId.HasValue)
            query = query.Where(p => p.ProductLocations.Any(pl => pl.LocationId == locationId.Value));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        query = query.Where(p => includeUncategorized || p.CategoryId != null);

        query = query.OrderBy(p => p.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize) .ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<List<Product>> GetAllForExportAsync(string? searchTerm, Guid? locationId, Guid? categoryId, bool? isActive, bool includeUncategorized, int maxRows, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.ProductLocations).ThenInclude(pl => pl.Location)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{searchTerm}%"));

        if (locationId.HasValue)
            query = query.Where(p => p.ProductLocations.Any(pl => pl.LocationId == locationId.Value));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        query = query.Where(p => includeUncategorized || p.CategoryId != null);

        var totalCount = await query.CountAsync(cancellationToken);
        if (totalCount > maxRows)
            throw new ValidationException(ExportLimits.ExceededMessage);

        return await query.OrderBy(p => p.Name).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
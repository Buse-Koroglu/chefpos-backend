using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Infastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ChefPos.Infastructure.Repositories;
public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;
    public CategoryRepository(ApplicationDbContext context) => _context = context;

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .Include(c => c.CategoryLocations)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<Category>> GetAllByLocationAsync(Guid locationId, bool includeInactive, CancellationToken cancellationToken)
    {
        var query = _context.Categories.Where(c => c.CategoryLocations.Any(cl => cl.LocationId == locationId));

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query.OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
    }

    public async Task SaveAllChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<(List<Category> Items, int TotalCount)> GetAllPagedAsync(
        string? searchTerm,
        Guid? locationId,
        bool? isActive,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .Include(c => c.CategoryLocations)
                .ThenInclude(cl => cl.Location)
            .Include(c => c.Products)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
            query = query.Where(c => EF.Functions.ILike(c.Name, $"%{searchTerm}%"));

        if (locationId.HasValue)
            query = query.Where(c => c.CategoryLocations.Any(cl => cl.LocationId == locationId.Value));

        if (isActive.HasValue)
            query = query.Where(c => c.IsActive == isActive.Value);

        query = query.OrderBy(c => c.Name);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await GetByIdAsync(id, cancellationToken);
        if (category is null)
        {
            throw new KeyNotFoundException("Kategori bulunamadı.");
        }
        _context.Categories.Remove(category);
    }
}
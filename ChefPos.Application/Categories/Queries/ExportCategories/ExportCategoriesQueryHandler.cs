using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Categories.Queries.ExportCategories;

public class ExportCategoriesQueryHandler : IRequestHandler<ExportCategoriesQuery, ExportFileResult>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IExcelExportService _excelExportService;

    public ExportCategoriesQueryHandler(ICategoryRepository categoryRepository, IUserRepository userRepository, ICurrentUserService currentUserService, IExcelExportService excelExportService)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _excelExportService = excelExportService;
    }

    public async Task<ExportFileResult> Handle(ExportCategoriesQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        var locationId = request.LocationId;
        var isSuperAdmin = actingUser.HasRole(Role.SUPER_ADMIN);
        if (!isSuperAdmin)
        {
            locationId = actingUser.LocationIdsForRole(Role.ADMIN).FirstOrDefault();
        }

        var categories = await _categoryRepository.GetAllForExportAsync(request.SearchTerm, locationId, request.IsActive, ExportLimits.MaxRows, cancellationToken);

        var columns = new List<ExportColumn<Category>>
        {
            new("Ad", c => c.Name),
            new("Aktif", c => c.IsActive),
            new("Lokasyonlar", c =>
            {
                var visibleLocations = isSuperAdmin
                    ? c.CategoryLocations
                    : c.CategoryLocations.Where(cl => cl.LocationId == locationId);
                return string.Join(", ", visibleLocations.Select(cl => cl.Location.Name));
            }),
            new("Ürün Sayısı", c => c.Products.Count),
        };

        var content = _excelExportService.Generate(categories, columns, "Kategoriler");
        var fileName = $"kategoriler_{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
        return new ExportFileResult(content, fileName);
    }
}

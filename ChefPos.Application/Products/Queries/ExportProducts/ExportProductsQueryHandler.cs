using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Products.Queries.ExportProducts;

public class ExportProductsQueryHandler : IRequestHandler<ExportProductsQuery, ExportFileResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IExcelExportService _excelExportService;

    public ExportProductsQueryHandler(IProductRepository productRepository, IUserRepository userRepository, ICurrentUserService currentUserService, IExcelExportService excelExportService)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _excelExportService = excelExportService;
    }

    public async Task<ExportFileResult> Handle(ExportProductsQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        var locationId = request.LocationId;
        var isSuperAdmin = actingUser.HasRole(Role.SUPER_ADMIN);
        if (!isSuperAdmin)
        {
            if (locationId.HasValue)
            {
                if (!actingUser.HasRoleAtLocation(Role.ADMIN, locationId.Value))
                    throw new ValidationException("Bu yerleşke için işlem yapma yetkiniz yok.");
            }
            else
            {
                locationId = actingUser.LocationIdsForRole(Role.ADMIN).FirstOrDefault();
            }
        }

        var products = await _productRepository.GetAllForExportAsync(request.SearchTerm, locationId, request.CategoryId, request.IsActive, request.IncludeUncategorized, ExportLimits.MaxRows, cancellationToken);

        var columns = new List<ExportColumn<Product>>
        {
            new("Ad", p => p.Name),
            new("Açıklama", p => p.Description),
            new("Fiyat", p => p.Price),
            new("Kategori", p => p.Category?.Name),
            new("Lokasyonlar", p =>
            {
                var visibleLocations = isSuperAdmin
                    ? p.ProductLocations
                    : p.ProductLocations.Where(pl => pl.LocationId == locationId);
                return string.Join(", ", visibleLocations.Select(pl => pl.Location.Name));
            }),
            new("Aktif", p => p.IsActive),
        };

        var content = _excelExportService.Generate(products, columns, "Ürünler");
        var fileName = $"urunler_{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
        return new ExportFileResult(content, fileName);
    }
}

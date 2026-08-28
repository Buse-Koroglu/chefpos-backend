using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Menus.Queries.ExportMenus;

public class ExportMenusQueryHandler : IRequestHandler<ExportMenusQuery, ExportFileResult>
{
    private readonly IMenuRepository _menuRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IExcelExportService _excelExportService;

    public ExportMenusQueryHandler(IMenuRepository menuRepository, IUserRepository userRepository, ICurrentUserService currentUserService, IExcelExportService excelExportService)
    {
        _menuRepository = menuRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _excelExportService = excelExportService;
    }

    public async Task<ExportFileResult> Handle(ExportMenusQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) && !actingUser.HasAccessToLocation(request.LocationId))
            throw new ValidationException("Bu yerleşke için işlem yapma yetkiniz yok.");

        var menus = await _menuRepository.GetAllForExportAsync(request.LocationId, request.IncludeInactive, ExportLimits.MaxRows, cancellationToken);

        var columns = new List<ExportColumn<Menu>>
        {
            new("Ad", m => m.Name),
            new("Açıklama", m => m.Description),
            new("Lokasyon", m => m.Location.Name),
            new("Aktif", m => m.IsActive),
            new("Ürünler", m => string.Join(", ", m.MenuProducts.OrderBy(mp => mp.DisplayOrder).Select(mp => mp.Product.Name))),
        };

        var content = _excelExportService.Generate(menus, columns, "Menüler");
        var fileName = $"menuler_{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
        return new ExportFileResult(content, fileName);
    }
}

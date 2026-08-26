using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Tables.Queries.ExportTables;

public class ExportTablesQueryHandler : IRequestHandler<ExportTablesQuery, ExportFileResult>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IExcelExportService _excelExportService;

    public ExportTablesQueryHandler(
        ITableRepository tableRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IExcelExportService excelExportService)
    {
        _tableRepository = tableRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _excelExportService = excelExportService;
    }

    public async Task<ExportFileResult> Handle(ExportTablesQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        var locationId = request.LocationId;
        if (!actingUser.HasRole(Role.SUPER_ADMIN))
        {
            locationId = actingUser.Locations.Select(l => l.LocationId).FirstOrDefault();
        }

        var tables = await _tableRepository.GetAllForExportAsync(
            request.SearchTerm, locationId, request.IsActive, ExportLimits.MaxRows, cancellationToken);

        var columns = new List<ExportColumn<Table>>
        {
            new("Masa No", t => t.TableNumber),
            new("Lokasyon", t => t.Location.Name),
            new("Aktif", t => t.IsActive),
        };

        var content = _excelExportService.Generate(tables, columns, "Masalar");
        var fileName = $"masalar_{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
        return new ExportFileResult(content, fileName);
    }
}

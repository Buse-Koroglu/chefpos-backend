using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using MediatR;

namespace ChefPos.Application.Locations.Queries.ExportLocations;

public class ExportLocationsQueryHandler : IRequestHandler<ExportLocationsQuery, ExportFileResult>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IExcelExportService _excelExportService;

    public ExportLocationsQueryHandler(ILocationRepository locationRepository, IUserRepository userRepository, IExcelExportService excelExportService)
    {
        _locationRepository = locationRepository;
        _userRepository = userRepository;
        _excelExportService = excelExportService;
    }

    public async Task<ExportFileResult> Handle(ExportLocationsQuery request, CancellationToken cancellationToken)
    {
        var locations = await _locationRepository.GetAllForExportAsync(request.SearchTerm, request.IsActive, ExportLimits.MaxRows, cancellationToken);

        var employeeCounts = await _userRepository.GetEmployeeCountsByLocationAsync(cancellationToken);

        var columns = new List<ExportColumn<Location>>
        {
            new("Ad", l => l.Name),
            new("Aktif", l => l.IsActive),
            new("Çalışan Sayısı", l => employeeCounts.FirstOrDefault(x => x.LocationId == l.Id).EmployeeCount),
        };

        var content = _excelExportService.Generate(locations, columns, "Yerleşkeler");
        var fileName = $"yerleskeler_{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
        return new ExportFileResult(content, fileName);
    }
}

using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.StockRequests.Queries.ExportStockRequests;

public class ExportStockRequestsQueryHandler : IRequestHandler<ExportStockRequestsQuery, ExportFileResult>
{
    private static readonly Dictionary<StockRequestStatus, string> StatusLabels = new()
    {
        [StockRequestStatus.PENDING] = "Beklemede",
        [StockRequestStatus.APPROVED] = "Onaylandı",
        [StockRequestStatus.REJECTED] = "Reddedildi",
    };

    private static readonly Dictionary<StockUnit, string> UnitLabels = new()
    {
        [StockUnit.KG] = "Kilogram",
        [StockUnit.LT] = "Litre",
    };

    private readonly IStockRequestRepository _stockRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IExcelExportService _excelExportService;

    public ExportStockRequestsQueryHandler(
        IStockRequestRepository stockRequestRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService,
        IExcelExportService excelExportService)
    {
        _stockRequestRepository = stockRequestRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _excelExportService = excelExportService;
    }

    public async Task<ExportFileResult> Handle(ExportStockRequestsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {currentUserId}");

        var locationId = request.LocationId;
        if (currentUser.HasRole(Role.ADMIN) && !currentUser.HasRole(Role.SUPER_ADMIN))
        {
            locationId = currentUser.Locations.Select(l => l.LocationId).FirstOrDefault();
        }

        var stockRequests = await _stockRequestRepository.GetAllForExportAsync(
            request.SearchTerm,
            locationId,
            request.Status,
            requestedByUserId: null,
            request.OnlyHistory,
            request.StartDate,
            request.EndDate,
            ExportLimits.MaxRows,
            cancellationToken);

        var columns = new List<ExportColumn<StockRequest>>
        {
            new("Ham Madde", sr => sr.Ingredient.Name),
            new("Birim", sr => UnitLabels.GetValueOrDefault(sr.Ingredient.Unit, sr.Ingredient.Unit.ToString())),
            new("Lokasyon", sr => sr.Location.Name),
            new("Talep Edilen Miktar", sr => sr.RequestedQuantity),
            new("Durum", sr => StatusLabels.GetValueOrDefault(sr.Status, sr.Status.ToString())),
            new("Talep Eden", sr => $"{sr.RequestedByUser.FirstName} {sr.RequestedByUser.LastName}".Trim()),
            new("Karar Veren", sr => sr.DecidedByUser != null ? $"{sr.DecidedByUser.FirstName} {sr.DecidedByUser.LastName}".Trim() : null),
            new("Ret Nedeni", sr => sr.RejectionReason),
            new("Talep Tarihi", sr => sr.CreatedAt),
            new("Karar Tarihi", sr => sr.DecidedAt),
            new("Onaylanan Birim Fiyat", sr => sr.ApprovedUnitPrice),
        };

        var content = _excelExportService.Generate(stockRequests, columns, "Stok Talepleri");
        var fileName = $"stok_talepleri_{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
        return new ExportFileResult(content, fileName);
    }
}

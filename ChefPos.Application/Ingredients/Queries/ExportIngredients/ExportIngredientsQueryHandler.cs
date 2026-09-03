using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Ingredients.Queries.ExportIngredients;

public class ExportIngredientsQueryHandler : IRequestHandler<ExportIngredientsQuery, ExportFileResult>
{
    private static readonly Dictionary<StockUnit, string> UnitLabels = new()
    {
        [StockUnit.KG] = "Kilogram",
        [StockUnit.LT] = "Litre",
    };

    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IExcelExportService _excelExportService;

    public ExportIngredientsQueryHandler(IIngredientRepository ingredientRepository, IUserRepository userRepository, ICurrentUserService currentUserService, IExcelExportService excelExportService)
    {
        _ingredientRepository = ingredientRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _excelExportService = excelExportService;
    }

    public async Task<ExportFileResult> Handle(ExportIngredientsQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        var locationId = request.LocationId;
        if (!actingUser.HasRole(Role.SUPER_ADMIN))
        {
            locationId = actingUser.LocationIdsForRole(Role.ADMIN).FirstOrDefault();
        }

        var ingredients = await _ingredientRepository.GetAllForExportAsync(request.SearchTerm, locationId, request.IsActive, ExportLimits.MaxRows, cancellationToken);

        var columns = new List<ExportColumn<Ingredient>>
        {
            new("Ad", i => i.Name),
            new("Birim", i => UnitLabels.GetValueOrDefault(i.Unit, i.Unit.ToString())),
            new("Son Alış Fiyatı", i => i.LatestUnitPrice),
            new("Ağırlıklı Ortalama Fiyat", i => i.WeightedAverageUnitPrice),
            new("Mevcut Stok", i => i.CurrentStock),
            new("Min. Stok Eşiği", i => i.MinStockThreshold),
            new("Eşik Altında mı", i => i.IsBellowThreshold),
            new("Aktif", i => i.IsActive),
            new("Lokasyon", i => i.Location.Name),
        };

        var content = _excelExportService.Generate(ingredients, columns, "Ham Maddeler");
        var fileName = $"ham_maddeler_{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
        return new ExportFileResult(content, fileName);
    }
}

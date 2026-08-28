using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Users.Queries.ExportUsers;

public class ExportUsersQueryHandler : IRequestHandler<ExportUsersQuery, ExportFileResult>
{
    private static readonly Dictionary<Role, string> RoleLabels = new()
    {
        [Role.ADMIN] = "Yönetici",
        [Role.CASHIER] = "Kasiyer",
        [Role.WAITER] = "Garson",
        [Role.STOCK_MANAGER] = "Stok Yöneticisi",
        [Role.INVENTORY_STAFF] = "Depo Görevlisi",
        [Role.KITCHEN] = "Mutfak",
        [Role.SUPER_ADMIN] = "Süper Yönetici",
    };

    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IExcelExportService _excelExportService;

    public ExportUsersQueryHandler(IUserRepository userRepository, ICurrentUserService currentUserService, IExcelExportService excelExportService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
        _excelExportService = excelExportService;
    }

    public async Task<ExportFileResult> Handle(ExportUsersQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");
        // adminler için ait oldukları location değerine göre locationId belirlenir.
        var locationId = request.LocationId;
        if (!actingUser.HasRole(Role.SUPER_ADMIN))
        {
            locationId = actingUser.Locations.Select(l => l.LocationId).FirstOrDefault();
        }

        var users = await _userRepository.GetAllForExportAsync(request.SearchTerm, request.Role, request.IsActive, locationId, ExportLimits.MaxRows, cancellationToken);

        var columns = new List<ExportColumn<User>>
        {
            new("Ad", u => u.FirstName),
            new("Soyad", u => u.LastName),
            new("Personel No", u => u.PersonalId),
            new("Roller", u => string.Join(", ", u.Roles.Select(r => RoleLabels.GetValueOrDefault(r, r.ToString())))),
            new("Aktif", u => u.IsActive),
            new("Lokasyonlar", u => string.Join(", ", u.Locations.Select(l => l.Location.Name))),
            new("Oluşturulma Tarihi", u => u.CreatedAt),
        };

        var content = _excelExportService.Generate(users, columns, "Personeller");
        var fileName = $"personeller_{DateTime.UtcNow:yyyy-MM-dd}.xlsx";
        return new ExportFileResult(content, fileName);
    }
}

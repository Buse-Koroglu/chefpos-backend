using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.StockMovements.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.StockMovements.Queries.GetStockMovementsPaged;

public class GetStockMovementsPagedQueryHandler
    : IRequestHandler<GetStockMovementsPagedQuery, PagedResult<StockMovementResponseDto>>
{
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetStockMovementsPagedQueryHandler(IStockMovementRepository stockMovementRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _stockMovementRepository = stockMovementRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<StockMovementResponseDto>> Handle(GetStockMovementsPagedQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {currentUserId}");

        if (!currentUser.HasRole(Role.ADMIN) && !currentUser.HasRole(Role.SUPER_ADMIN) && !currentUser.HasRole(Role.STOCK_MANAGER) && !currentUser.HasRole(Role.INVENTORY_STAFF))
        {
            throw new ValidationException("Stok hareketlerini görüntüleme yetkiniz yok.");
        }

        if (!currentUser.HasRole(Role.SUPER_ADMIN) && request.LocationId.HasValue && !currentUser.HasAccessToLocation(request.LocationId.Value))
        {
            throw new ValidationException("Bu yerleşkeye erişim yetkiniz yok.");
        }

        var (items, totalCount) = await _stockMovementRepository.GetAllPagedAsync(request.IngredientId,request.LocationId, request.Type, request.PageNumber, request.PageSize, cancellationToken);

        var stockMovementResponseDtos = items.Select(StockMovementResponseDto.FromEntity).ToList();

        return new PagedResult<StockMovementResponseDto>
        {
            Items = stockMovementResponseDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}

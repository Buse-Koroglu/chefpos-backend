using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.StockRequests.Queries.GetStockRequestPaged;

public class GetPagedStockRequestsQueryHandler
    : IRequestHandler<GetPagedStockRequestsQuery, PagedResult<AdminStockRequestResponseDto>>
{
    private readonly IStockRequestRepository _stockRequestRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetPagedStockRequestsQueryHandler(IStockRequestRepository stockRequestRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _stockRequestRepository = stockRequestRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<AdminStockRequestResponseDto>> Handle(GetPagedStockRequestsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");
        
        var isPrivilegedUser = currentUser.HasRole(Role.ADMIN) || currentUser.HasRole(Role.SUPER_ADMIN) || currentUser.HasRole(Role.STOCK_MANAGER);

        var mustFilterByOwner = request.OnlyMyRequests || !isPrivilegedUser;

        Guid? requestedByUserId = mustFilterByOwner ? currentUserId : null;

        var locationId = request.LocationId;
        if (currentUser.HasRole(Role.ADMIN) && !currentUser.HasRole(Role.SUPER_ADMIN))
        {
            locationId = currentUser.Locations.Select(l => l.LocationId).FirstOrDefault();
        }

        var (items, totalCount) = await _stockRequestRepository.GetAllPagedAsync(request.SearchTerm, locationId, request.Status, requestedByUserId, request.OnlyHistory, request.StartDate, request.EndDate, request.PageNumber, request.PageSize, cancellationToken);

        var adminStockRequestResponseDtos = items.Select(AdminStockRequestResponseDto.FromEntity).ToList();

        return new PagedResult<AdminStockRequestResponseDto>
        {
            Items = adminStockRequestResponseDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
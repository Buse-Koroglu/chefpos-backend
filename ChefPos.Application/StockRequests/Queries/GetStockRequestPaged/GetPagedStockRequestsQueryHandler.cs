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

    public GetPagedStockRequestsQueryHandler(
        IStockRequestRepository stockRequestRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _stockRequestRepository = stockRequestRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<AdminStockRequestResponseDto>> Handle(
        GetPagedStockRequestsQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        var currentUser = await _userRepository.GetByIdAsync(currentUserId, cancellationToken);

        if (currentUser is null)
        {
            throw new NotFoundException($"Kullanıcı bulunamadı: {currentUserId}");
        }
        
        var isPrivilegedUser = currentUser.HasRole(Role.ADMIN) || currentUser.HasRole(Role.STOCK_MANAGER);

        var mustFilterByOwner = request.OnlyMyRequests || !isPrivilegedUser;

        Guid? requestedByUserId = mustFilterByOwner ? currentUserId : null;

        var (items, totalCount) = await _stockRequestRepository.GetAllPagedAsync(
            request.SearchTerm,
            request.LocationId,
            request.Status,
            requestedByUserId,
            request.OnlyHistory,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(AdminStockRequestResponseDto.FromEntity).ToList();

        return new PagedResult<AdminStockRequestResponseDto>
        {
            Items = dtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Tables.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Tables.Queries.GetTablesPaged;

public class GetTablesPagedQueryHandler : IRequestHandler<GetTablesPagedQuery, PagedResult<TableResponseDto>>
{
    private readonly ITableRepository _tableRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetTablesPagedQueryHandler(ITableRepository tableRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _tableRepository = tableRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<TableResponseDto>> Handle(GetTablesPagedQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        var locationId = request.LocationId;
        if (!actingUser.HasRole(Role.SUPER_ADMIN))
        {
            locationId = actingUser.Locations.Select(l => l.LocationId).FirstOrDefault();
        }

        var (tables, totalCount) = await _tableRepository.GetAllPagedAsync(
            request.SearchTerm, locationId, request.IsActive, request.PageNumber, request.PageSize, cancellationToken);

        return new PagedResult<TableResponseDto>
        {
            Items = tables.Select(TableResponseDto.FromEntity).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
        };
    }
}

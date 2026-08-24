using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Users.DTOs;
using ChefPos.Application.Users.Queries.GetAllUsers;
using ChefPos.Domain.Enums;
using MediatR;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedResult<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAllUsersQueryHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<UserResponseDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        var locationId = request.LocationId;
        if (!actingUser.HasRole(Role.SUPER_ADMIN))
        {
            locationId = actingUser.Locations.Select(l => l.LocationId).FirstOrDefault();
        }

        var (users, totalCount) = await _userRepository.GetAllPagedAsync(
            request.SearchTerm, request.Role, request.IsActive, locationId, request.PageNumber, request.PageSize, cancellationToken);

        var isSuperAdmin = actingUser.HasRole(Role.SUPER_ADMIN);

        var items = users.Select(u =>
        {
            var dto = UserResponseDto.FromEntity(u);
            if (!isSuperAdmin)
            {
                dto.LocationIds = dto.LocationIds.Where(id => id == locationId).ToList();
            }
            return dto;
        }).ToList();

        return new PagedResult<UserResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
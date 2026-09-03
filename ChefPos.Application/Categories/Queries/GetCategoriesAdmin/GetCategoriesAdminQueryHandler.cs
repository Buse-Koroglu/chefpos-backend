using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Categories.Queries.GetCategoriesAdmin;


public class GetCategoriesAdminQueryHandler : IRequestHandler<GetCategoriesAdminQuery, PagedResult<CategoryAdminResponseDto>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetCategoriesAdminQueryHandler(ICategoryRepository categoryRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<CategoryAdminResponseDto>> Handle(GetCategoriesAdminQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        var locationId = request.LocationId;
        if (!actingUser.HasRole(Role.SUPER_ADMIN))
            locationId = actingUser.LocationIdsForRole(Role.ADMIN).FirstOrDefault();

        var (categories, totalCount) = await _categoryRepository.GetAllPagedAsync(request.SearchTerm, locationId, request.IsActive, request.PageNumber, request.PageSize, cancellationToken);

        var isSuperAdmin = actingUser.HasRole(Role.SUPER_ADMIN);

        var items = categories.Select(c =>
        {
            var visibleLocations = isSuperAdmin
                ? c.CategoryLocations
                : c.CategoryLocations.Where(cl => cl.LocationId == locationId);

            return new CategoryAdminResponseDto
            {
                Id = c.Id,
                Name = c.Name,
                Icon = c.Icon,
                IsActive = c.IsActive,
                LocationIds = visibleLocations.Select(cl => cl.LocationId).ToList(),
                LocationNames = visibleLocations.Select(cl => cl.Location.Name).ToList(),
                ProductCount = c.Products.Count
            };
        }).ToList();

        return new PagedResult<CategoryAdminResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
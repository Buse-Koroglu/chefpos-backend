using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using MediatR;

namespace ChefPos.Application.Categories.Queries.GetCategoriesAdmin;


public class GetCategoriesAdminQueryHandler : IRequestHandler<GetCategoriesAdminQuery, PagedResult<CategoryAdminResponseDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesAdminQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<CategoryAdminResponseDto>> Handle(GetCategoriesAdminQuery request, CancellationToken cancellationToken)
    {
        var (categories, totalCount) = await _categoryRepository.GetAllPagedAsync(
            request.SearchTerm, request.LocationId, request.IsActive, request.PageNumber, request.PageSize, cancellationToken);

        var items = categories.Select(c => new CategoryAdminResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            Icon = c.Icon,
            IsActive = c.IsActive,
            LocationId = c.LocationId,
            LocationName = c.Location.Name,
            ProductCount = c.Products.Count
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
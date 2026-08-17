using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Pagination;
using MediatR;

namespace ChefPos.Application.Categories.Queries.GetCategoriesAdmin;

public class GetCategoriesAdminQuery : IRequest<PagedResult<CategoryAdminResponseDto>>
{
    public string? SearchTerm { get; }
    public Guid? LocationId { get; }
    public bool? IsActive { get; }
    public int PageNumber { get; }
    public int PageSize { get; }

    public GetCategoriesAdminQuery(string? searchTerm, Guid? locationId, bool? isActive, int pageNumber = 1, int pageSize = 20)
    {
        SearchTerm = searchTerm;
        LocationId = locationId;
        IsActive = isActive;
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize is < 1 or > 100 ? 20 : pageSize;
    }
}
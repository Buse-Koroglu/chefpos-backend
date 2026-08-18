using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Queries.GetProductsPaged;

public class GetProductsPagedQuery : IRequest<PagedResult<ProductAdminResponseDto>>
{
    public string? SearchTerm { get; }
    public Guid? LocationId { get; }
    public Guid? CategoryId { get; }
    public bool? IsActive { get; }
    public int PageNumber { get; }
    public int PageSize { get; }

    public GetProductsPagedQuery(string? searchTerm, Guid? locationId, Guid? categoryId, bool? isActive, int pageNumber = 1, int pageSize = 20)
    {
        SearchTerm = searchTerm;
        LocationId = locationId;
        CategoryId = categoryId;
        IsActive = isActive;
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize is < 1 or > 100 ? 20 : pageSize;
    }
}

using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Tables.DTOs;
using MediatR;

namespace ChefPos.Application.Tables.Queries.GetTablesPaged;

public class GetTablesPagedQuery : IRequest<PagedResult<TableResponseDto>>
{
    public string? SearchTerm { get; }
    public Guid? LocationId { get; }
    public bool? IsActive { get; }
    public int PageNumber { get; }
    public int PageSize { get; }

    public GetTablesPagedQuery(string? searchTerm, Guid? locationId, bool? isActive, int pageNumber = 1, int pageSize = 20)
    {
        SearchTerm = searchTerm;
        LocationId = locationId;
        IsActive = isActive;
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize is < 1 or > 100 ? 20 : pageSize;
    }
}

using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Locations.DTOs;
using MediatR;

namespace ChefPos.Application.Locations.Queries.GetLocationsPaged;

public class GetLocationsPagedQuery : IRequest<PagedResult<LocationResponseDto>>
{
    public string? SearchTerm { get; }
    public bool? IsActive { get; }
    public int PageNumber { get; }
    public int PageSize { get; }

    public GetLocationsPagedQuery(string? searchTerm, bool? isActive, int pageNumber = 1, int pageSize = 20)
    {
        SearchTerm = searchTerm;
        IsActive = isActive;
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize is < 1 or > 100 ? 20 : pageSize;
    }
}
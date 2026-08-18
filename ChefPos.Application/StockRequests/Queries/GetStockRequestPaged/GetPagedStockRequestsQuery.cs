using ChefPos.Application.Common.Pagination;
using ChefPos.Application.StockRequests.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.StockRequests.Queries.GetStockRequestPaged;

public class GetPagedStockRequestsQuery : IRequest<PagedResult<AdminStockRequestResponseDto>>
{
    public string? SearchTerm { get; set; }
    public Guid? LocationId { get; set; }
    public StockRequestStatus? Status { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    

    public GetPagedStockRequestsQuery(
        string? searchTerm, 
        Guid? locationId, 
        StockRequestStatus? status, 
        int pageNumber = 1, 
        int pageSize = 10)
    {
        SearchTerm = searchTerm;
        LocationId = locationId;
        Status = status;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
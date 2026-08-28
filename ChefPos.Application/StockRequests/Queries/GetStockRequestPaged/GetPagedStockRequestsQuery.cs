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
    public bool OnlyMyRequests { get; set; }
    public bool OnlyHistory { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;


    public GetPagedStockRequestsQuery(string? searchTerm, Guid? locationId, StockRequestStatus? status, bool onlyMyRequests = false, bool onlyHistory = false, DateTime? startDate = null, DateTime? endDate = null, int pageNumber = 1, int pageSize = 10)
    {
        SearchTerm = searchTerm;
        LocationId = locationId;
        Status = status;
        OnlyMyRequests = onlyMyRequests;
        OnlyHistory = onlyHistory;
        StartDate = startDate;
        EndDate = endDate;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
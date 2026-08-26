using ChefPos.Application.Common.Export;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.StockRequests.Queries.ExportStockRequests;

public class ExportStockRequestsQuery : IRequest<ExportFileResult>
{
    public string? SearchTerm { get; }
    public Guid? LocationId { get; }
    public StockRequestStatus? Status { get; }
    public bool OnlyHistory { get; }
    public DateTime? StartDate { get; }
    public DateTime? EndDate { get; }

    public ExportStockRequestsQuery(
        string? searchTerm,
        Guid? locationId,
        StockRequestStatus? status,
        bool onlyHistory,
        DateTime? startDate,
        DateTime? endDate)
    {
        SearchTerm = searchTerm;
        LocationId = locationId;
        Status = status;
        OnlyHistory = onlyHistory;
        StartDate = startDate;
        EndDate = endDate;
    }
}

using ChefPos.Application.StockRequests.DTOs;

namespace ChefPos.Application.StockRequests.Queries.GetStockManagerDashboardStats;
using MediatR;

public class GetStockManagerDashboardStatsQuery : IRequest<StockManagerDashboardStatsDto>
{
    public Guid LocationId { get; }

    public GetStockManagerDashboardStatsQuery(Guid locationId)
    {
        LocationId = locationId;
    }
}
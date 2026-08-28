using ChefPos.Application.StockRequests.DTOs;

namespace ChefPos.Application.StockRequests.Queries.GetInventoryDashboardStats;
using MediatR;

public class GetInventoryDashboardStatsQuery : IRequest<InventoryDashboardStatsDto>
{
    public Guid? LocationId { get; }

    public GetInventoryDashboardStatsQuery(Guid locationId)
    {
        LocationId = locationId;
    }
}
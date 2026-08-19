using ChefPos.Application.StockRequests.DTOs;

namespace ChefPos.Application.StockRequests.Queries.GetInventoryDashboardStats;
using MediatR;

public sealed record GetInventoryDashboardStatsQuery( Guid? LocationId ) : IRequest<InventoryDashboardStatsDto>;
using ChefPos.Application.StockRequests.DTOs;

namespace ChefPos.Application.StockRequests.Queries.GetStockManagerDashboardStats;
using MediatR;

public sealed record GetStockManagerDashboardStatsQuery(Guid LocationId) : IRequest<StockManagerDashboardStatsDto>;

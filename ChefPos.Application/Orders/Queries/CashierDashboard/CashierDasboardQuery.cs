using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Queries.CashierDashboard;

public class GetCashierDashboardQuery : IRequest<CashierDashboardResponseDto>
{
    public Guid LocationId { get; set; }
 
    public GetCashierDashboardQuery(Guid locationId)
    {
        LocationId = locationId;
    }
}

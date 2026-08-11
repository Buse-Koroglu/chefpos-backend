using ChefPos.Application.Orders.DTOs;
using MediatR;

namespace ChefPos.Application.Orders.Queries.GetWeeklyReveune;
public class GetWeeklyRevenueQuery : IRequest<WeeklyRevenueResponseDto>
{
    public Guid LocationId { get; set; }
 
    public GetWeeklyRevenueQuery(Guid locationId)
    {
        LocationId = locationId;
    }
}

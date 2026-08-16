namespace ChefPos.Application.Dashboard.Admin.Queries.GetDashboardSummary;
using MediatR;

public class GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>
{
    public Guid LocationId { get; set; }

    public GetDashboardSummaryQuery(Guid locationId)
    {
        LocationId = locationId;
    }
}
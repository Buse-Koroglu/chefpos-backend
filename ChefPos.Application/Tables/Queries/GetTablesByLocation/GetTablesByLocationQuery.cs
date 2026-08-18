using ChefPos.Application.Tables.DTOs;
using MediatR;

namespace ChefPos.Application.Tables.Queries.GetTablesByLocation;

public class GetTablesByLocationQuery : IRequest<List<TableResponseDto>>
{
    public Guid LocationId { get; }
    public bool IncludeInactive { get; }

    public GetTablesByLocationQuery(Guid locationId, bool includeInactive)
    {
        LocationId = locationId;
        IncludeInactive = includeInactive;
    }
}

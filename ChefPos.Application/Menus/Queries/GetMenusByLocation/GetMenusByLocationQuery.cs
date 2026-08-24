using ChefPos.Application.Menus.DTOs;
using MediatR;

namespace ChefPos.Application.Menus.Queries.GetMenusByLocation;

public class GetMenusByLocationQuery : IRequest<List<MenuResponseDto>>
{
    public Guid LocationId { get; set; }
    public bool IncludeInactive { get; set; }

    public GetMenusByLocationQuery(Guid locationId, bool includeInactive)
    {
        LocationId = locationId;
        IncludeInactive = includeInactive;
    }
}
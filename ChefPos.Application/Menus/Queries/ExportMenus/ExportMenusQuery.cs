using ChefPos.Application.Common.Export;
using MediatR;

namespace ChefPos.Application.Menus.Queries.ExportMenus;

public class ExportMenusQuery : IRequest<ExportFileResult>
{
    public Guid LocationId { get; }
    public bool IncludeInactive { get; }

    public ExportMenusQuery(Guid locationId, bool includeInactive)
    {
        LocationId = locationId;
        IncludeInactive = includeInactive;
    }
}

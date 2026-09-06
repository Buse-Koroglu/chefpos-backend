using ChefPos.Application.Common.Export;
using MediatR;

namespace ChefPos.Application.Locations.Queries.ExportLocations;

public class ExportLocationsQuery : IRequest<ExportFileResult>
{
    public string? SearchTerm { get; }
    public bool? IsActive { get; }

    public ExportLocationsQuery(string? searchTerm, bool? isActive)
    {
        SearchTerm = searchTerm;
        IsActive = isActive;
    }
}

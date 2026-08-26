using ChefPos.Application.Common.Export;
using MediatR;

namespace ChefPos.Application.Categories.Queries.ExportCategories;

public class ExportCategoriesQuery : IRequest<ExportFileResult>
{
    public string? SearchTerm { get; }
    public Guid? LocationId { get; }
    public bool? IsActive { get; }

    public ExportCategoriesQuery(string? searchTerm, Guid? locationId, bool? isActive)
    {
        SearchTerm = searchTerm;
        LocationId = locationId;
        IsActive = isActive;
    }
}

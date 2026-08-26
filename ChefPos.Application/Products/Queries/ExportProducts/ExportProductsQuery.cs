using ChefPos.Application.Common.Export;
using MediatR;

namespace ChefPos.Application.Products.Queries.ExportProducts;

public class ExportProductsQuery : IRequest<ExportFileResult>
{
    public string? SearchTerm { get; }
    public Guid? LocationId { get; }
    public Guid? CategoryId { get; }
    public bool? IsActive { get; }
    public bool IncludeUncategorized { get; }

    public ExportProductsQuery(string? searchTerm, Guid? locationId, Guid? categoryId, bool? isActive, bool includeUncategorized = false)
    {
        SearchTerm = searchTerm;
        LocationId = locationId;
        CategoryId = categoryId;
        IsActive = isActive;
        IncludeUncategorized = includeUncategorized;
    }
}

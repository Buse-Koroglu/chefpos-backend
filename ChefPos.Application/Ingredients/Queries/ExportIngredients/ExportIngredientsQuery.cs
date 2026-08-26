using ChefPos.Application.Common.Export;
using MediatR;

namespace ChefPos.Application.Ingredients.Queries.ExportIngredients;

public class ExportIngredientsQuery : IRequest<ExportFileResult>
{
    public string? SearchTerm { get; }
    public Guid? LocationId { get; }
    public bool? IsActive { get; }

    public ExportIngredientsQuery(string? searchTerm, Guid? locationId, bool? isActive)
    {
        SearchTerm = searchTerm;
        LocationId = locationId;
        IsActive = isActive;
    }
}

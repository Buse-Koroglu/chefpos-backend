using ChefPos.Application.Categories.DTOs;
using MediatR;

namespace ChefPos.Application.Categories.Queries.GetCategories;

public class GetCategoriesQuery : IRequest<List<CategoryResponseDto>>
{
public Guid LocationId { get; set; }
public bool IncludeInactives { get; set; }

public GetCategoriesQuery(Guid locationId, bool includeInactives)
{
    LocationId = locationId;
    IncludeInactives = includeInactives;
}
}
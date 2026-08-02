using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Queries.GetIngredients;

public class GetIngredientsQuery : IRequest<List<IngredientResponseDto>>
{
    public Guid LocationId { get;  set; }
    public bool IncludeInactive { get;  set; }

    public GetIngredientsQuery(Guid locationId, bool includeInactive)
    {
        LocationId = locationId;
        IncludeInactive = includeInactive;
    }
}
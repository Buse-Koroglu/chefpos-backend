using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Queries.GetLowStockIngredients;

public class GetLowStockIngredientsQuery : IRequest<List<IngredientResponseDto>>
{
    public Guid LocationId { get; set; }
 
    public GetLowStockIngredientsQuery(Guid locationId)
    {
        LocationId = locationId;
    }
}

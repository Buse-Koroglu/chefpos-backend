using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Queries.GetIngredientById;

public class GetIngredientByIdQuery : IRequest<IngredientResponseDto>
{
    public Guid IngredientId { get; set; }

    public GetIngredientByIdQuery(Guid ingredientId)
    {
        IngredientId = ingredientId;
    }
}
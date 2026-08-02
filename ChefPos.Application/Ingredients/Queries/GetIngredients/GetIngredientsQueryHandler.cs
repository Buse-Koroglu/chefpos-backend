using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Queries.GetIngredients;

public class GetIngredientsQueryHandler : IRequestHandler<GetIngredientsQuery,List<IngredientResponseDto>>
{

    private readonly IIngredientRepository _ingredientRepository;

    public GetIngredientsQueryHandler(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<List<IngredientResponseDto>> Handle(GetIngredientsQuery request, CancellationToken cancellationToken)
    {
        var ingredients =
            await _ingredientRepository.GetAllByLocationAsync(request.LocationId, request.IncludeInactive,cancellationToken);
        return ingredients.Select(IngredientResponseDto.FromEntity).ToList();
        
    }
}
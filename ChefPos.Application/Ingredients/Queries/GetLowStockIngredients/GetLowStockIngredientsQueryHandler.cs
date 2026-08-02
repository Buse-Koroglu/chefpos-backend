using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Application.Ingredients.Queries.GetLowStockIngredients;
using MediatR;

public class GetLowStockIngredientsQueryHandler : IRequestHandler<GetLowStockIngredientsQuery, List<IngredientResponseDto>>
{
    private readonly IIngredientRepository _ingredientRepository;

    public GetLowStockIngredientsQueryHandler(IIngredientRepository ingredientRepository) => _ingredientRepository = ingredientRepository;

    public async Task<List<IngredientResponseDto>> Handle(GetLowStockIngredientsQuery request, CancellationToken cancellationToken)
    {
        var ingredients = await _ingredientRepository.GetLowStockAsync(request.LocationId, cancellationToken);

        return ingredients.Select(IngredientResponseDto.FromEntity).ToList();
    }
}
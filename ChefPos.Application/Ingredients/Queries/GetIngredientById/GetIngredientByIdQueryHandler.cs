using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Queries.GetIngredientById;

public class GetIngredientByIdQueryHandler : IRequestHandler<GetIngredientByIdQuery, IngredientResponseDto>
{
    private readonly IIngredientRepository _ingredientRepository;

    public GetIngredientByIdQueryHandler(IIngredientRepository ingredientRepository) => _ingredientRepository = ingredientRepository;

    public async Task<IngredientResponseDto> Handle(GetIngredientByIdQuery request, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken).OrThrowNotFoundAsync($"Ham madde bulunamadı: {request.IngredientId}");
        return IngredientResponseDto.FromEntity(ingredient);
    }
}
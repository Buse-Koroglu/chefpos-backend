using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.UpdateIngredientMinStockThreshold;

public class UpdateIngredientMinStockThresholdCommandHandler : IRequestHandler<UpdateIngredientMinStockThresholdCommand, IngredientResponseDto>
{
    private readonly IIngredientRepository _ingredientRepository;

    public UpdateIngredientMinStockThresholdCommandHandler(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<IngredientResponseDto> Handle(UpdateIngredientMinStockThresholdCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken).OrThrowNotFoundAsync($"Ham madde bulunamadı: {request.IngredientId}");

        ingredient.UpdateMinStockThreshold(request.MinStockThreshold);

        await _ingredientRepository.SaveAllChangesAsync(cancellationToken);

        return IngredientResponseDto.FromEntity(ingredient);
    }
}
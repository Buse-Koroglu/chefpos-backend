using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands.UpdateIngredient;

public class UpdateIngredientCommandHandler : IRequestHandler<UpdateIngredientCommand, IngredientResponseDto>
{
    private readonly IIngredientRepository _ingredientRepository;

    public UpdateIngredientCommandHandler(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<IngredientResponseDto> Handle(UpdateIngredientCommand request, CancellationToken cancellationToken)
    {
        var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId, cancellationToken)
            .OrThrowNotFoundAsync($"Ham madde bulunamadı: {request.IngredientId}");
        
        if (!ingredient.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await _ingredientRepository.GetAllByLocationAsync(ingredient.LocationId, includeInactive: true, cancellationToken);
            if (existing.Any(i => i.Id != ingredient.Id && i.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException("Bu isimde bir ham madde bu yerleşkede zaten mevcut.");
            }
        }

        ingredient.UpdateDetails(request.Name);

        await _ingredientRepository.SaveAllChangesAsync(cancellationToken);

        return IngredientResponseDto.FromEntity(ingredient);
    }
}
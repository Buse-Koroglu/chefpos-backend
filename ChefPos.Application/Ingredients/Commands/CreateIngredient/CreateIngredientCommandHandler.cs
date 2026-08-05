using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Ingredients.Commands;
using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Domain.Entities;
using MediatR;

public class CreateIngredientCommandHandler : IRequestHandler<CreateIngredientCommand, IngredientResponseDto>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILocationRepository _locationRepository;

    public CreateIngredientCommandHandler(IIngredientRepository ingredientRepository, ILocationRepository locationRepository)
    {
        _ingredientRepository = ingredientRepository;
        _locationRepository = locationRepository;
    }

    public async Task<IngredientResponseDto> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken);
        if (location is null)
        {
            throw new NotFoundException("Yerleşke bulunamadı.");
        }
        
        var existing = await _ingredientRepository.GetAllByLocationAsync(request.LocationId, includeInactive: true, cancellationToken);
        if (existing.Any(i => i.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("Bu isimde bir ham madde bu yerleşkede zaten mevcut.");
        }

        var ingredient = new Ingredient(request.Name, request.Unit, request.UnitPrice, request.LocationId, request.InitialStock, request.MinStockThreshold);

        await _ingredientRepository.AddAsync(ingredient, cancellationToken);
        await _ingredientRepository.SaveAllChangesAsync(cancellationToken);

        return IngredientResponseDto.FromEntity(ingredient);
    }
}
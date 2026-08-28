using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Ingredients.Commands;
using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Domain.Entities;
using MediatR;
public class CreateIngredientCommandHandler : IRequestHandler<CreateIngredientCommand, List<IngredientResponseDto>>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly ILocationRepository _locationRepository;

    public CreateIngredientCommandHandler(IIngredientRepository ingredientRepository, ILocationRepository locationRepository)
    {
        _ingredientRepository = ingredientRepository;
        _locationRepository = locationRepository;
    }

    public async Task<List<IngredientResponseDto>> Handle(CreateIngredientCommand request, CancellationToken cancellationToken)
    {
        if (request.LocationIds is null || request.LocationIds.Count == 0)
            throw new ValidationException("En az bir yerleşke seçilmelidir.");

        var distinctLocationIds = request.LocationIds.Distinct().ToList();
        var createdIngredients = new List<Ingredient>();

        foreach (var locationId in distinctLocationIds)
        {
            var location = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
            if (location is null)
            {
                throw new NotFoundException($"Yerleşke bulunamadı: {locationId}");
            }

            var existing = await _ingredientRepository.GetAllByLocationAsync(locationId, includeInactive: true, cancellationToken);
            if (existing.Any(i => i.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ValidationException($"'{location.Name}' yerleşkesinde bu isimde bir ham madde zaten mevcut.");
            }

            var ingredient = new Ingredient(request.Name, request.Unit, request.UnitPrice, locationId, request.InitialStock, request.MinStockThreshold);
            createdIngredients.Add(ingredient);
        }

        foreach (var ingredient in createdIngredients)
            await _ingredientRepository.AddAsync(ingredient, cancellationToken);
        

        await _ingredientRepository.SaveAllChangesAsync(cancellationToken);

        return createdIngredients.Select(IngredientResponseDto.FromEntity).ToList();
    }
}
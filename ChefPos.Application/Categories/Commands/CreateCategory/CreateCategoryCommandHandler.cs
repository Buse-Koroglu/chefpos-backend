using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using MediatR;

namespace ChefPos.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponseDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, ILocationRepository locationRepository)
    {
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<CategoryResponseDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        foreach (var locationId in request.LocationIds.Distinct())
        {
            await _locationRepository.GetByIdAsync(locationId, cancellationToken)
                .OrThrowNotFoundAsync($"Yerleşke bulunamadı: {locationId}");
        }

        var category = new Category(request.Name, request.LocationIds, request.Icon);

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);

        return CategoryResponseDto.FromEntity(category);
    }
}
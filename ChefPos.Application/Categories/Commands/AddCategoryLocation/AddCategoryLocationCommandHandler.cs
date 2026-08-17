using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using MediatR;

namespace ChefPos.Application.Categories.Commands.AddCategoryLocation;

public class AddCategoryLocationCommandHandler : IRequestHandler<AddCategoryLocationCommand, CategoryResponseDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;

    public AddCategoryLocationCommandHandler(ICategoryRepository categoryRepository, ILocationRepository locationRepository)
    {
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<CategoryResponseDto> Handle(AddCategoryLocationCommand request, CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken)
            .OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            .OrThrowNotFoundAsync($"Kategori bulunamadı: {request.CategoryId}");

        category.AddLocation(request.LocationId);
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);

        return CategoryResponseDto.FromEntity(category);
    }
}

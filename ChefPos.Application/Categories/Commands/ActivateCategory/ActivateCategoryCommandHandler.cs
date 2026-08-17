using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using MediatR;

namespace ChefPos.Application.Categories.Commands.ActivateCategory;

public class ActivateCategoryCommandHandler : IRequestHandler<ActivateCategoryCommand, CategoryResponseDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;

    public ActivateCategoryCommandHandler(ICategoryRepository categoryRepository, ILocationRepository locationRepository)
    {
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<CategoryResponseDto> Handle(ActivateCategoryCommand request, CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken).OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken).OrThrowNotFoundAsync($"Kategori bulunamadı: {request.Id}");

        if (!category.BelongsToLocation(request.LocationId))
        {
            throw new ForbiddenException("Bu işleme yetkiniz bulunmamaktır.");
        }

        category.ActivateCategory();
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);

        return CategoryResponseDto.FromEntity(category);
    }
}
using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using MediatR;

namespace ChefPos.Application.Categories.Commands.RemoveCategory;

public class DeactivateCategoryCommandHandler : IRequestHandler<DeactivateCategoryCommand,CategoryResponseDto>
{
    
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILocationRepository _locationRepository;

    public DeactivateCategoryCommandHandler(ICategoryRepository categoryRepository, ILocationRepository locationRepository)
    {
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<CategoryResponseDto> Handle(DeactivateCategoryCommand request, CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId,cancellationToken).OrThrowNotFoundAsync($"Yerleşke Bulunamadı : {request.LocationId}");
        
        var category = await _categoryRepository.GetByIdAsync(request.Id,cancellationToken).OrThrowNotFoundAsync($"Kategori bulunamadı : {request.Id}");
        
        if (category.LocationId != request.LocationId)
        {
            throw new UnauthorizedAccessException("Bu işleme yetkiniz bulunmamaktır.");
        }
        
        category.DeactivateCategory();
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);

        return CategoryResponseDto.FromEntity(category);
    }
    
    
    
}
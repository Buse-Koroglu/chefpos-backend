using ChefPos.Application.Categories.DTOs;
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
        var location = await _locationRepository.GetByIdAsync(request.LocationId,cancellationToken);
        if (location is null)
        {
            throw new KeyNotFoundException("Yerleşke bulunamadı.");
        }
        
        var category = await _categoryRepository.GetByIdAsync(request.Id,cancellationToken);
        if (category is null)
        {
            throw new KeyNotFoundException("Kategori bulunamadı.");
        }
        
        
        if (category.LocationId != request.LocationId)
        {
            throw new UnauthorizedAccessException("Bu işleme yetkiniz bulunmamaktır.");
        }
        
        category.DeactivateCategory();
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);

        return CategoryResponseDto.FromEntity(category);
    }
    
    
    
}
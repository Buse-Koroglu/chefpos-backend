using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using MediatR;

namespace ChefPos.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand,CategoryResponseDto>
{
    private readonly ICategoryRepository  _categoryRepository;
    private readonly ILocationRepository _locationRepository;
    

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository, ILocationRepository locationRepository)
    {
        _categoryRepository = categoryRepository;
        _locationRepository = locationRepository;
    }

    public async Task<CategoryResponseDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var location = await _locationRepository.GetByIdAsync(request.LocationId,cancellationToken);
        if (location is null)
        {
            throw new KeyNotFoundException("Yerleşke bulunamadı.");
        }

        var category = new Category(request.Name,request.LocationId,request.Icon);

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);

        return CategoryResponseDto.FromEntity(category);
    }
    
}
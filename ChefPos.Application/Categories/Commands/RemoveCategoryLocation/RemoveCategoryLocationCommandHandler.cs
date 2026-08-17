using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using MediatR;

namespace ChefPos.Application.Categories.Commands.RemoveCategoryLocation;

public class RemoveCategoryLocationCommandHandler : IRequestHandler<RemoveCategoryLocationCommand, CategoryResponseDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public RemoveCategoryLocationCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponseDto> Handle(RemoveCategoryLocationCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken)
            .OrThrowNotFoundAsync($"Kategori bulunamadı: {request.CategoryId}");

        category.RemoveLocation(request.LocationId);
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);

        return CategoryResponseDto.FromEntity(category);
    }
}

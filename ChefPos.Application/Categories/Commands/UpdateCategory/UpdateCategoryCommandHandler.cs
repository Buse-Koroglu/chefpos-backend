using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Interfaces;
using MediatR;

namespace ChefPos.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand,CategoryResponseDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponseDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            throw new KeyNotFoundException("Kategori bulunamadı.");
        }
        category.UpdateDetails(request.Name,request.Icon);
        await _categoryRepository.SaveAllChangesAsync(cancellationToken);
        return CategoryResponseDto.FromEntity(category);
    }
}
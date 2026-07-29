using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Common.Interfaces;
using MediatR;

namespace ChefPos.Application.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery,List<CategoryResponseDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryResponseDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllByLocationAsync(request.LocationId,request.IncludeInactives,cancellationToken);
        return categories.Select(CategoryResponseDto.FromEntity).ToList();
    }
}
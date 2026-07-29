using ChefPos.Application.Categories.DTOs;
using MediatR;

namespace ChefPos.Application.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery : IRequest<CategoryResponseDto>
{
    public Guid CategoryId { get; set; }
    public GetCategoryByIdQuery(Guid categoryId)
    {
        CategoryId = categoryId;
    }
    
}
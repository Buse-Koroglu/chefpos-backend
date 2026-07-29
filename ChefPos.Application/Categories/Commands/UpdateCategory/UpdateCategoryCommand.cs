using ChefPos.Application.Categories.DTOs;
using MediatR;

namespace ChefPos.Application.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<CategoryResponseDto>
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; } = default!;
    public string? Icon  { get; set; }
    
    public UpdateCategoryCommand(Guid categoryId, string name, string icon)
    {
        CategoryId = categoryId;
        Name = name;
        Icon = icon;
    }
}
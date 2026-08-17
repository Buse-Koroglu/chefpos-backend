using ChefPos.Application.Categories.DTOs;
using MediatR;

public class CreateCategoryCommand : IRequest<CategoryResponseDto>
{
    public string Name { get; set; } = default!;
    public string? Icon { get; set; }
    public List<Guid> LocationIds { get; set; } = new();

    public CreateCategoryCommand(string name, string? icon, List<Guid> locationIds)
    {
        Name = name;
        Icon = icon;
        LocationIds = locationIds;
    }
}
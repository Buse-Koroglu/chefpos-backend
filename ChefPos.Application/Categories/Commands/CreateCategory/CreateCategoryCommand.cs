using ChefPos.Application.Categories.DTOs;
using MediatR;

namespace ChefPos.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<CategoryResponseDto>
{
    public string Name { get; set; } = default!;
    public string? Icon  { get; set; }
    public Guid LocationId { get; set; }

    public CreateCategoryCommand(string name, string icon, Guid locationId)
    {
        Name = name;
        Icon = icon;
        LocationId = locationId;
    }
}
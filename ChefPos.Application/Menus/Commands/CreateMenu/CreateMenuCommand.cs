using ChefPos.Application.Menus.DTOs;
using MediatR;

namespace ChefPos.Application.Menus.Commands.CreateMenu;

public class CreateMenuCommand : IRequest<MenuResponseDto>
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public Guid LocationId { get; set; }

    public CreateMenuCommand(string name, string? description, Guid locationId)
    {
        Name = name;
        Description = description;
        LocationId = locationId;
    }
}
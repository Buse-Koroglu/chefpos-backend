using ChefPos.Application.Menus.DTOs;
using MediatR;

namespace ChefPos.Application.Menus.Commands.UpdateMenu;

public class UpdateMenuCommand : IRequest<MenuResponseDto>
{
    public Guid MenuId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }

    public UpdateMenuCommand(Guid menuId, string name, string? description)
    {
        MenuId = menuId;
        Name = name;
        Description = description;
    }
}
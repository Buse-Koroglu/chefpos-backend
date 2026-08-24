using ChefPos.Application.Menus.DTOs;
using MediatR;

namespace ChefPos.Application.Menus.Commands.ActivateMenu;

public class ActivateMenuCommand : IRequest<MenuResponseDto>
{
    public Guid MenuId { get; set; }

    public ActivateMenuCommand(Guid menuId)
    {
        MenuId = menuId;
    }
}
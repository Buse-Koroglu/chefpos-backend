using ChefPos.Application.Menus.DTOs;
using MediatR;

namespace ChefPos.Application.Menus.Commands.DeactivateMenu;

public class DeactivateMenuCommand : IRequest<MenuResponseDto>
{
    public Guid MenuId { get; set; }

    public DeactivateMenuCommand(Guid menuId)
    {
        MenuId = menuId;
    }
}
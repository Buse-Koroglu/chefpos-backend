using ChefPos.Application.Menus.DTOs;
using MediatR;

namespace ChefPos.Application.Menus.Commands.RemoveProductFromMenu;

public class RemoveProductFromMenuCommand : IRequest<MenuResponseDto>
{
    public Guid MenuId { get; set; }
    public Guid ProductId { get; set; }

    public RemoveProductFromMenuCommand(Guid menuId, Guid productId)
    {
        MenuId = menuId;
        ProductId = productId;
    }
}
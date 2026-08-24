using ChefPos.Application.Menus.DTOs;
using MediatR;

namespace ChefPos.Application.Menus.Commands.AddProductToMenu;

public class AddProductToMenuCommand : IRequest<MenuResponseDto>
{
    public Guid MenuId { get; set; }
    public Guid ProductId { get; set; }

    public AddProductToMenuCommand(Guid menuId, Guid productId)
    {
        MenuId = menuId;
        ProductId = productId;
    }
}
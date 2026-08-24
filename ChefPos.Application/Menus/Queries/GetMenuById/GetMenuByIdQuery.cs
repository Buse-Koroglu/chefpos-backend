using ChefPos.Application.Menus.DTOs;
using MediatR;

namespace ChefPos.Application.Menus.Queries.GetMenuById;

public class GetMenuByIdQuery : IRequest<MenuResponseDto>
{
    public Guid MenuId { get; set; }

    public GetMenuByIdQuery(Guid menuId)
    {
        MenuId = menuId;
    }
}
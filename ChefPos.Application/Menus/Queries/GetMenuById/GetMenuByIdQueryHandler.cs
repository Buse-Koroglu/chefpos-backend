using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Menus.DTOs;
using MediatR;

namespace ChefPos.Application.Menus.Queries.GetMenuById;

public class GetMenuByIdQueryHandler : IRequestHandler<GetMenuByIdQuery, MenuResponseDto>
{
    private readonly IMenuRepository _menuRepository;

    public GetMenuByIdQueryHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<MenuResponseDto> Handle(GetMenuByIdQuery request, CancellationToken cancellationToken)
    {
        var menu = await _menuRepository.GetByIdAsync(request.MenuId, cancellationToken).OrThrowNotFoundAsync("Menü bulunamadı.");

        return MenuResponseDto.FromEntity(menu);
    }
}
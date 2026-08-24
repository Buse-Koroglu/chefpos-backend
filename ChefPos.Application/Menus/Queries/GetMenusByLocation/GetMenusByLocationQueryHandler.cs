using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Menus.DTOs;
using MediatR;

namespace ChefPos.Application.Menus.Queries.GetMenusByLocation;

public class GetMenusByLocationQueryHandler : IRequestHandler<GetMenusByLocationQuery, List<MenuResponseDto>>
{
    private readonly IMenuRepository _menuRepository;

    public GetMenusByLocationQueryHandler(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<List<MenuResponseDto>> Handle(GetMenusByLocationQuery request, CancellationToken cancellationToken)
    {
        var menus = await _menuRepository.GetAllByLocationAsync(request.LocationId, request.IncludeInactive, cancellationToken);
        return menus.Select(MenuResponseDto.FromEntity).ToList();
    }
}
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Ingredients.Queries.GetIngredientsPaged;

public class GetIngredientsPagedQueryHandler : IRequestHandler<GetIngredientsPagedQuery, PagedResult<IngredientAdminResponseDto>>
{
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetIngredientsPagedQueryHandler(IIngredientRepository ingredientRepository, IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _ingredientRepository = ingredientRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<PagedResult<IngredientAdminResponseDto>> Handle(GetIngredientsPagedQuery request, CancellationToken cancellationToken)
    {
        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        var locationId = request.LocationId;
        if (!actingUser.HasRole(Role.SUPER_ADMIN))
        {
            locationId = actingUser.LocationIdsForRole(Role.ADMIN).FirstOrDefault();
        }

        var (ingredients, totalCount) = await _ingredientRepository.GetAllPagedAsync(request.SearchTerm, locationId, request.IsActive, request.PageNumber, request.PageSize, cancellationToken);

        var items = ingredients.Select(i => new IngredientAdminResponseDto
        {
            Id = i.Id,
            Name = i.Name,
            Unit = i.Unit,
            LatestUnitPrice = i.LatestUnitPrice,
            WeightedAverageUnitPrice = i.WeightedAverageUnitPrice,
            CurrentStock = i.CurrentStock,
            MinStockThreshold = i.MinStockThreshold,
            IsBelowThreshold = i.IsBellowThreshold,
            IsActive = i.IsActive,
            LocationId = i.LocationId,
            LocationName = i.Location.Name
        }).ToList();

        return new PagedResult<IngredientAdminResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
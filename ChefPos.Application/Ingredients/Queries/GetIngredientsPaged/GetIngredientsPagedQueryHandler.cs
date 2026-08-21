using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Ingredients.DTOs;
using MediatR;

namespace ChefPos.Application.Ingredients.Queries.GetIngredientsPaged;

public class GetIngredientsPagedQueryHandler : IRequestHandler<GetIngredientsPagedQuery, PagedResult<IngredientAdminResponseDto>>
{
    private readonly IIngredientRepository _ingredientRepository;

    public GetIngredientsPagedQueryHandler(IIngredientRepository ingredientRepository)
    {
        _ingredientRepository = ingredientRepository;
    }

    public async Task<PagedResult<IngredientAdminResponseDto>> Handle(GetIngredientsPagedQuery request, CancellationToken cancellationToken)
    {
        var (ingredients, totalCount) = await _ingredientRepository.GetAllPagedAsync(
            request.SearchTerm, request.LocationId, request.IsActive, request.PageNumber, request.PageSize, cancellationToken);

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
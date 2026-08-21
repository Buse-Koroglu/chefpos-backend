using ChefPos.Application.Common.Pagination;
using ChefPos.Application.StockMovements.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.StockMovements.Queries.GetStockMovementsPaged;

public class GetStockMovementsPagedQuery : IRequest<PagedResult<StockMovementResponseDto>>
{
    public Guid? IngredientId { get; set; }
    public Guid? LocationId { get; set; }
    public StockMovementType? Type { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public GetStockMovementsPagedQuery(
        Guid? ingredientId,
        Guid? locationId,
        StockMovementType? type,
        int pageNumber = 1,
        int pageSize = 20)
    {
        IngredientId = ingredientId;
        LocationId = locationId;
        Type = type;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}

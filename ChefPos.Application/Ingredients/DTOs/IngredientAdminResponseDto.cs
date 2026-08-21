using ChefPos.Domain.Enums;

namespace ChefPos.Application.Ingredients.DTOs;
public class IngredientAdminResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public StockUnit Unit { get; set; }
    public decimal? LatestUnitPrice { get; set; }
    public decimal WeightedAverageUnitPrice { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal MinStockThreshold { get; set; }
    public bool IsBelowThreshold { get; set; }
    public bool IsActive { get; set; }
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = default!;
}
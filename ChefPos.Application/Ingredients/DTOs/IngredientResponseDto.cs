using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.Ingredients.DTOs;

public class IngredientResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public StockUnit Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal CurrentStock  { get; set; }
    public decimal MinStockThreshold { get; set; }
    public bool IsBelowThreshold { get; set; }
    public bool IsActive { get; set; }
    public Guid LocationId { get; set; }
    
    public static IngredientResponseDto FromEntity(Ingredient ingredient)
    {
        return new IngredientResponseDto
        {
            Id = ingredient.Id,
            Name = ingredient.Name,
            Unit = ingredient.Unit,
            UnitPrice = ingredient.UnitPrice,
            CurrentStock = ingredient.CurrentStock,
            MinStockThreshold = ingredient.MinStockThreshold,
            IsBelowThreshold = ingredient.IsBellowThreshold,
            IsActive = ingredient.IsActive,
            LocationId = ingredient.LocationId
        };
    }
}


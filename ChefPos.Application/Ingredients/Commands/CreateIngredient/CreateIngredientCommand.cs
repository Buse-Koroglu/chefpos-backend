using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands;

public class CreateIngredientCommand : IRequest<IngredientResponseDto>
{
    public string Name { get; set; } = default!;
    public StockUnit Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public Guid LocationId { get; set; }
    public decimal InitialStock { get; set; }
    public decimal MinStockThreshold { get; set; }
}
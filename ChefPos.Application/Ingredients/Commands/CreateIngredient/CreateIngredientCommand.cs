using ChefPos.Application.Ingredients.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Ingredients.Commands;

public class CreateIngredientCommand : IRequest<List<IngredientResponseDto>>
{
    public string Name { get; set; } = default!;
    public StockUnit Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public List<Guid> LocationIds { get; set; } = new();
    public decimal InitialStock { get; set; }
    public decimal MinStockThreshold { get; set; }
}
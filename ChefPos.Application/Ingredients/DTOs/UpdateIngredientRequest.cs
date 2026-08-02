namespace ChefPos.Application.Ingredients.DTOs;

public class UpdateIngredientRequest
{
    public string Name { get; set; } = default!;
    public decimal UnitPrice { get; set; }
}
namespace ChefPos.Application.Products.DTOs;

public class AddProductIngredientRequestDto
{
    public string Name { get; set; } = default!;
    public decimal UnitPrice { get; set; }

}
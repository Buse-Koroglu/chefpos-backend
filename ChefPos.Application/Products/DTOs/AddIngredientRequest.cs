namespace ChefPos.Application.Products.DTOs;

public class AddIngredientRequest
{
    public Guid IngredientId { get; set; }
    public decimal QuantityPerServing { get; set; }
}
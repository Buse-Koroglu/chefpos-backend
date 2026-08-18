namespace ChefPos.Application.Products.DTOs;

public class AddIngredientRequest
{
    public Guid LocationId { get; set; }
    public Guid IngredientId { get; set; }
    public decimal QuantityPerServing { get; set; }
}

namespace ChefPos.Application.Ingredients.DTOs;

public class RecordIngredientPurchaseRequest
{
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Note { get; set; }
}
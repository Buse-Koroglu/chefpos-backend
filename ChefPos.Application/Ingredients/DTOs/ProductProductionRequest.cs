namespace ChefPos.Application.Ingredients.DTOs;
public class ProductProductionRequest
{
    public Guid ProductId { get; set; }
    public Guid LocationId { get; set; }
    public int Quantity { get; set; }
    public string? Note { get; set; }
}
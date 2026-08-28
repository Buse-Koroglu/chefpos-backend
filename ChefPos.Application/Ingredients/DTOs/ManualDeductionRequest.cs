namespace ChefPos.Application.Ingredients.DTOs;

public class ManualDeductionRequest
{
    public decimal Quantity { get; set; }
    public string Note { get; set; } = default!;
}
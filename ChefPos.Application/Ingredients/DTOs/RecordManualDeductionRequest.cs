namespace ChefPos.Application.Ingredients.DTOs;

public class RecordManualDeductionRequest
{
    public decimal Quantity { get; set; }
    public string Note { get; set; } = default!;
}
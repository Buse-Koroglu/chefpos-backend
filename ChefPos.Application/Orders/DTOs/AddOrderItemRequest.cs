namespace ChefPos.Application.Orders.DTOs;

public class AddOrderItemRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
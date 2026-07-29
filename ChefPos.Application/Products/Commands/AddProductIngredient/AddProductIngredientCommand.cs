using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.AddProductIngredient;

public class AddProductIngredientCommand : IRequest<ProductResponseDto>
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public decimal UnitPrice { get; set; }

    public AddProductIngredientCommand(Guid productId, string name, decimal unitPrice)
    {
        ProductId = productId;
        Name = name;
        UnitPrice = unitPrice;    
    }
    
}
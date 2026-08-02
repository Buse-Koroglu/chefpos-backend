using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.RemoveProductIngredient;

public class RemoveProductIngredientCommand : IRequest<ProductResponseDto>
{
    public Guid ProductId { get; set; }
    public Guid ProductItemId { get; set; }

    public RemoveProductIngredientCommand(Guid productId, Guid productItemId)
    {
        ProductId = productId;
        ProductItemId = productItemId;
    }
    
}
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.RemoveProductIngredient;

public class RemoveProductIngredientCommand : IRequest<ProductResponseDto>
{
    public Guid ProductId { get; set; }
    public Guid LocationId { get; set; }
    public Guid ProductItemId { get; set; }

    public RemoveProductIngredientCommand(Guid productId, Guid locationId, Guid productItemId)
    {
        ProductId = productId;
        LocationId = locationId;
        ProductItemId = productItemId;
    }

}

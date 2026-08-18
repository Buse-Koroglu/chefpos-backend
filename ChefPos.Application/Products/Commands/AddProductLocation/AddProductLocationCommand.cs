using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.AddProductLocation;

public class AddProductLocationCommand : IRequest<ProductResponseDto>
{
    public Guid ProductId { get; }
    public Guid LocationId { get; }

    public AddProductLocationCommand(Guid productId, Guid locationId)
    {
        ProductId = productId;
        LocationId = locationId;
    }
}

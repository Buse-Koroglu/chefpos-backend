using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.RemoveProductLocation;

public class RemoveProductLocationCommand : IRequest<ProductResponseDto>
{
    public Guid ProductId { get; }
    public Guid LocationId { get; }

    public RemoveProductLocationCommand(Guid productId, Guid locationId)
    {
        ProductId = productId;
        LocationId = locationId;
    }
}

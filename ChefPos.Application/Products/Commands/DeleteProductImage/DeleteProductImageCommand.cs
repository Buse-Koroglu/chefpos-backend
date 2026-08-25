using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.DeleteProductImage;

public class DeleteProductImageCommand : IRequest<ProductResponseDto>
{
    public Guid ProductId { get; }

    public DeleteProductImageCommand(Guid productId)
    {
        ProductId = productId;
    }
}

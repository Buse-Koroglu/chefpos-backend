using ChefPos.Application.Common.Models;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.SetProductImage;

public class SetProductImageCommand : IRequest<ProductResponseDto>
{
    public Guid ProductId { get; }
    public FileUploadRequest File { get; }

    public SetProductImageCommand(Guid productId, FileUploadRequest file)
    {
        ProductId = productId;
        File = file;
    }
}

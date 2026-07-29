using ChefPos.Application.Categories.DTOs;
using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<ProductResponseDto>
{
    public Guid ProductId { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public UpdateProductCommand(Guid productId, string name, string? description, string? imageUrl)
    {
        ProductId = productId;
        Name = name;
        Description = description;
        ImageUrl = imageUrl;
    }
    
}
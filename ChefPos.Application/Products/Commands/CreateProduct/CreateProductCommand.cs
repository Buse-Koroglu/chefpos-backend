using ChefPos.Application.Products.DTOs;
using MediatR;

namespace ChefPos.Application.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<ProductResponseDto>
{
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public Guid CategoryId { get; set; }
    public List<Guid> LocationIds { get; set; } = new();
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
}
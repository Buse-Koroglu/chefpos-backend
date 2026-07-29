using ChefPos.Domain.Entities;

namespace ChefPos.Application.Products.DTOs;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public Guid LocationId { get; set; }
    public Guid CategoryId { get; set; }
    public List<ProductItemResponseDto> ProductItems { get; set; } = new();

    public static ProductResponseDto FromEntity(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            LocationId = product.LocationId,
            CategoryId = product.CategoryId,
            ProductItems = product.ProductItems.Select(ProductItemResponseDto.FromEntity).ToList()
        };
    }
    
    public class ProductItemResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public decimal UnitPrice { get; set; }

        public static ProductItemResponseDto FromEntity(ProductItem item)
        {
            return new ProductItemResponseDto
            {
                Id = item.Id,
                Name = item.Name,
                UnitPrice = item.UnitPrice
            };
        }
    }
}
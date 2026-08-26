using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.Products.DTOs;

public class ProductResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailable { get; set; }
    public Guid? CategoryId { get; set; }
    public List<Guid> LocationIds { get; set; } = new();
    public List<ProductLocationRecipeDto> Locations { get; set; } = new();

    public static ProductResponseDto FromEntity(Product product, Guid? locationId = null)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            IsActive = product.IsActive,
            IsAvailable = ComputeIsAvailable(product, locationId),
            CategoryId = product.CategoryId,
            LocationIds = product.LocationIds.ToList(),
            Locations = product.ProductLocations.Select(ProductLocationRecipeDto.FromEntity).ToList()
        };
    }

    private static bool ComputeIsAvailable(Product product, Guid? locationId)
    {
        if (locationId is null)
        {
            return true;
        }

        var productLocation = product.ProductLocations.FirstOrDefault(pl => pl.LocationId == locationId.Value);
        if (productLocation is null || productLocation.ProductItems.Count == 0)
        {
            return true;
        }

        return productLocation.ProductItems.All(pi =>
            pi.Ingredient.IsActive && pi.Ingredient.CurrentStock >= pi.QuantityPerServing);
    }
}

public class ProductLocationRecipeDto
{
    public Guid LocationId { get; set; }
    public List<ProductItemResponseDto> Ingredients { get; set; } = new();

    public static ProductLocationRecipeDto FromEntity(ProductLocation productLocation)
    {
        return new ProductLocationRecipeDto
        {
            LocationId = productLocation.LocationId,
            Ingredients = productLocation.ProductItems.Select(ProductItemResponseDto.FromEntity).ToList()
        };
    }
}

public class ProductItemResponseDto
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = default!;
    public StockUnit Unit { get; set; }
    public decimal QuantityPerServing { get; set; }

    public static ProductItemResponseDto FromEntity(ProductItem item)
    {
        return new ProductItemResponseDto
        {
            Id = item.Id,
            IngredientId = item.IngredientId,
            IngredientName = item.Ingredient.Name,
            Unit = item.Ingredient.Unit,
            QuantityPerServing = item.QuantityPerServing
        };
    }
}

using ChefPos.Domain.Entities;

namespace ChefPos.Application.Products;

public static class ProductAvailability
{
    public static bool IsAvailable(Product product, Guid? locationId)
    {
        if (locationId is null)
            return true;

        var productLocation = product.ProductLocations.FirstOrDefault(pl => pl.LocationId == locationId.Value);
        if (productLocation is null || productLocation.ProductItems.Count == 0)
            return true;

        return productLocation.ProductItems.All(pi =>
            pi.Ingredient.IsActive && pi.Ingredient.CurrentStock >= pi.QuantityPerServing);
    }
}

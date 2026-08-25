using ChefPos.Domain.Entities;

namespace ChefPos.Domain.Tests;

public class ProductTest
{
    private const string Name = "Mock Product";
    private const string Description = "Mock Desc";
    private const decimal Price = 100;
    private  const string ImageUrl = "Mock ImageUrl";

    [Fact]
    public void ShouldCreateConstructorWithValidParameters()
    {
        var locationId = Guid.NewGuid();
        var product = new Product(Name, Price, Guid.NewGuid(), new[] { locationId }, Description, ImageUrl);
        Assert.Equal(Name, product.Name);
        Assert.NotNull(product);
        Assert.True(product.IsActive);
        Assert.Single(product.LocationIds);
        Assert.Contains(locationId, product.LocationIds);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ShouldThrowAnExceptionWithEmptyProductName(string? name)
    {
        Assert.Throws<ArgumentException>(() =>
            new Product(name!, Price, Guid.NewGuid(), new[] { Guid.NewGuid() }));
    }


    [Fact]
    public void ShouldThrowAnExceptionWithNegativePrice()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Product(Name, -5, Guid.NewGuid(), new[] { Guid.NewGuid() }));
    }

    [Fact]
    public void ShouldAcceptWithZeroPrice()
    {
        var product = new Product(Name, 0, Guid.NewGuid(), new[] { Guid.NewGuid() });

        Assert.Equal(0, product.Price);
    }

    [Fact]
    public void ShouldThrowAnExceptionWithEmptyLocationList()
    {
        Assert.Throws<ArgumentException>(() =>
            new Product(Name, Price, Guid.NewGuid(), Array.Empty<Guid>()));
    }

    [Fact]
    public void ShouldStoreMultipleLocationsWhenGiven()
    {
        var locationId1 = Guid.NewGuid();
        var locationId2 = Guid.NewGuid();
        var product = new Product(Name, Price, Guid.NewGuid(), new[] { locationId1, locationId2 });

        Assert.Equal(2, product.LocationIds.Count());
        Assert.Contains(locationId1, product.LocationIds);
        Assert.Contains(locationId2, product.LocationIds);
    }

    [Fact]
    public void ShouldReturnTrueWhenProductBelongsToLocation()
    {
        var locationId = Guid.NewGuid();
        var product = new Product(Name, Price, Guid.NewGuid(), new[] { locationId });

        Assert.True(product.BelongsToLocation(locationId));
    }

    [Fact]
    public void ShouldReturnFalseWhenProductDoesNotBelongToLocation()
    {
        var product = new Product(Name, Price, Guid.NewGuid(), new[] { Guid.NewGuid() });

        Assert.False(product.BelongsToLocation(Guid.NewGuid()));
    }

    [Fact]
    public void ShouldThrowWhenAddingIngredientToLocationProductDoesNotBelongTo()
    {
        var product = new Product(Name, Price, Guid.NewGuid(), new[] { Guid.NewGuid() });

        Assert.Throws<InvalidOperationException>(() =>
            product.AddIngredient(Guid.NewGuid(), Guid.NewGuid(), 1));
    }

    [Fact]
    public void ShouldBeIsActiveFalseAfterDeactivate()
    {
        var product = new Product(Name, Price, Guid.NewGuid(), new[] { Guid.NewGuid() });
        product.DeactivateProduct();
        Assert.False(product.IsActive);
    }

    [Fact]
    public void ShouldBeIsNotActiveFalseAfterActivate()
    {
        var product = new Product(Name, Price, Guid.NewGuid(), new[] { Guid.NewGuid() });
        product.DeactivateProduct();
        product.ActivateProduct();
        Assert.True(product.IsActive);
    }

    [Fact]
    public void ShouldUpdateImageUrlAfterSetImage()
    {
        var product = new Product(Name, Price, Guid.NewGuid(), new[] { Guid.NewGuid() });
        product.SetImage("/uploads/products/new-image.webp");
        Assert.Equal("/uploads/products/new-image.webp", product.ImageUrl);
    }

    [Fact]
    public void ShouldClearImageUrlWhenSetImageCalledWithNull()
    {
        var product = new Product(Name, Price, Guid.NewGuid(), new[] { Guid.NewGuid() }, Description, ImageUrl);
        product.SetImage(null);
        Assert.Null(product.ImageUrl);
    }

    [Fact]
    public void ShouldNotChangeImageUrlWhenUpdateDetailsIsCalled()
    {
        var product = new Product(Name, Price, Guid.NewGuid(), new[] { Guid.NewGuid() }, Description, ImageUrl);
        product.UpdateDetails("Updated Name", "Updated Desc");
        Assert.Equal(ImageUrl, product.ImageUrl);
    }

}

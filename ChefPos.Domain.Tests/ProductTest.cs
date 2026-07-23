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
        var product = new Product(Name, Price, Guid.NewGuid(),Guid.NewGuid(),Description,ImageUrl);
        Assert.Equal(Name, product.Name);
        Assert.NotNull(product);
        Assert.True(product.IsActive);
        
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ShouldThrowAnExceptionWithEmptyProductName(string? name)
    {
        Assert.Throws<ArgumentException>(() =>
            new Product(name!, Price, Guid.NewGuid(), Guid.NewGuid()));
    }
    
    
    [Fact]
    public void ShouldThrowAnExceptionWithNegativePrice()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Product(Name, -5, Guid.NewGuid(), Guid.NewGuid()));
    }
 
    [Fact]
    public void ShouldAcceptWithZeroPrice()
    {
        var product = new Product(Name, 0, Guid.NewGuid(), Guid.NewGuid());
 
        Assert.Equal(0, product.Price);
    }
    
    [Fact]
    public void ShouldBeIsActiveFalseAfterDeactivate()
    {
        var product = new Product(Name, Price, Guid.NewGuid(), Guid.NewGuid());
        product.DeactivateProduct();
        Assert.False(product.IsActive);
    }
    
    [Fact]
    public void ShouldBeIsNotActiveFalseAfterActivate()
    {
        var product = new Product(Name, Price, Guid.NewGuid(), Guid.NewGuid());
        product.DeactivateProduct();
        product.ActivateProduct();
        Assert.True(product.IsActive);
    }
    
}
using ChefPos.Domain.Entities;

namespace ChefPos.Domain.Tests;

public class CategoryTest
{
    [Fact]
    public void CreateConstructorWhenAllParametersAreValid()
    {
        var category = new Category("Mock Category", Guid.NewGuid(),"Mock Icon");
        Assert.Equal("Mock Category", category.Name);
        Assert.Equal("Mock Icon", category.Icon);
        Assert.True(category.IsActive);
    }

    [Fact]
    public void IconShouldBeNullWhenItIsNotGiven()
    {
        var category = new Category("Mock Category", Guid.NewGuid());
        Assert.Null(category.Icon);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ThrowAnErrorWithEmptyCategoryName(string? name)
    {
        Assert.Throws<ArgumentException>(() =>
            new Category(name!, Guid.NewGuid()));
    }
    
    [Fact]
    public void ShouldBeIsActiveIsFalseWhenDeactivated()
    {
        var category = new Category("Mock Category", Guid.NewGuid());
 
        category.DeactivateCategory();
 
        Assert.False(category.IsActive);
    }
 
    [Fact]
    public void ShouldBeIsActiveIsTrueWhenActivated()
    {
        var category = new Category("Mock Category", Guid.NewGuid());
        category.DeactivateCategory();
 
        category.ActivateCategory();
 
        Assert.True(category.IsActive);
    }
}
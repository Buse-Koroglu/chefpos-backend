using ChefPos.Domain.Entities;

namespace ChefPos.Domain.Tests;

public class LocationTest
{
    private const string Name = "Mock Location";
    [Fact]
    public void ShouldCreateConstructorWithValidParameters()
    {
        var location = new Location(Name);
        Assert.NotNull(location);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ShouldThrowAnErrorWithEmptyLocationName(string? name)
    {
        Assert.Throws<ArgumentException>(() =>
            new Location(name!));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ShouldThrowAnErrorRenameWithEmptyLocationName(string? name)
    {
        var location = new Location(Name);
 
        Assert.Throws<ArgumentException>(() => location.Rename(name!));
    }
    
    [Fact]
    public void ShouldUpdatedCorrectlyWhenNameIsValid()
    {
        var location = new Location(Name);
 
        location.Rename("Mock Office");
 
        Assert.Equal("Mock Office", location.Name);
    }
 
    [Fact]
    public void ShouldBeIsActiveIsFalseWhenDeactivated()
    {
        var location = new Location(Name);
 
        location.DeactivateLocation();
 
        Assert.False(location.IsActive);
    }
 
    [Fact]
    public void ShouldBeIsActiveIsTrueWhenActivated()
    {
        var location = new Location(Name);
        location.DeactivateLocation();
 
        location.ActivateLocation();
 
        Assert.True(location.IsActive);
    }
}
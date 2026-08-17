using ChefPos.Domain.Entities;
namespace ChefPos.Domain.Tests;

public class CategoryTest
{
    [Fact]
    public void CreateConstructorWhenAllParametersAreValid()
    {
        var locationId = Guid.NewGuid();
        var category = new Category("Mock Category", new[] { locationId }, "Mock Icon");

        Assert.Equal("Mock Category", category.Name);
        Assert.Equal("Mock Icon", category.Icon);
        Assert.True(category.IsActive);
        Assert.Single(category.LocationIds);
        Assert.Contains(locationId, category.LocationIds);
    }

    [Fact]
    public void IconShouldBeNullWhenItIsNotGiven()
    {
        var category = new Category("Mock Category", new[] { Guid.NewGuid() });
        Assert.Null(category.Icon);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ThrowAnErrorWithEmptyCategoryName(string? name)
    {
        Assert.Throws<ArgumentException>(() =>
            new Category(name!, new[] { Guid.NewGuid() }));
    }

    [Fact]
    public void ThrowAnErrorWithEmptyLocationList()
    {
        Assert.Throws<ArgumentException>(() =>
            new Category("Mock Category", Array.Empty<Guid>()));
    }

    [Fact]
    public void ThrowAnErrorWithNullLocationList()
    {
        Assert.Throws<ArgumentException>(() =>
            new Category("Mock Category", null!));
    }

    [Fact]
    public void ShouldStoreDistinctLocationsWhenDuplicatesGiven()
    {
        var locationId = Guid.NewGuid();
        var category = new Category("Mock Category", new[] { locationId, locationId });

        Assert.Single(category.LocationIds);
    }

    [Fact]
    public void ShouldStoreMultipleLocationsWhenGiven()
    {
        var locationId1 = Guid.NewGuid();
        var locationId2 = Guid.NewGuid();
        var category = new Category("Mock Category", new[] { locationId1, locationId2 });

        Assert.Equal(2, category.LocationIds.Count());
        Assert.Contains(locationId1, category.LocationIds);
        Assert.Contains(locationId2, category.LocationIds);
    }

    [Fact]
    public void ShouldAddLocationWhenNotAlreadyAssigned()
    {
        var category = new Category("Mock Category", new[] { Guid.NewGuid() });
        var newLocationId = Guid.NewGuid();

        category.AddLocation(newLocationId);

        Assert.Contains(newLocationId, category.LocationIds);
        Assert.Equal(2, category.LocationIds.Count());
    }

    [Fact]
    public void ShouldThrowWhenAddingAlreadyAssignedLocation()
    {
        var locationId = Guid.NewGuid();
        var category = new Category("Mock Category", new[] { locationId });

        Assert.Throws<InvalidOperationException>(() => category.AddLocation(locationId));
    }

    [Fact]
    public void ShouldRemoveLocationWhenMoreThanOneExists()
    {
        var locationId1 = Guid.NewGuid();
        var locationId2 = Guid.NewGuid();
        var category = new Category("Mock Category", new[] { locationId1, locationId2 });

        category.RemoveLocation(locationId1);

        Assert.DoesNotContain(locationId1, category.LocationIds);
        Assert.Single(category.LocationIds);
    }

    [Fact]
    public void ShouldThrowWhenRemovingLastLocation()
    {
        var locationId = Guid.NewGuid();
        var category = new Category("Mock Category", new[] { locationId });

        Assert.Throws<InvalidOperationException>(() => category.RemoveLocation(locationId));
    }

    [Fact]
    public void ShouldReturnTrueWhenCategoryBelongsToLocation()
    {
        var locationId = Guid.NewGuid();
        var category = new Category("Mock Category", new[] { locationId });

        Assert.True(category.BelongsToLocation(locationId));
    }

    [Fact]
    public void ShouldReturnFalseWhenCategoryDoesNotBelongToLocation()
    {
        var category = new Category("Mock Category", new[] { Guid.NewGuid() });

        Assert.False(category.BelongsToLocation(Guid.NewGuid()));
    }

    [Fact]
    public void ShouldBeIsActiveIsFalseWhenDeactivated()
    {
        var category = new Category("Mock Category", new[] { Guid.NewGuid() });

        category.DeactivateCategory();

        Assert.False(category.IsActive);
    }

    [Fact]
    public void ShouldBeIsActiveIsTrueWhenActivated()
    {
        var category = new Category("Mock Category", new[] { Guid.NewGuid() });
        category.DeactivateCategory();

        category.ActivateCategory();

        Assert.True(category.IsActive);
    }
}
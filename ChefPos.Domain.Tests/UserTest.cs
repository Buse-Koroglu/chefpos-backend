using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Domain.Tests;

public class UserTest
{
    private const string PersonalId = "E001";
    private const string Name = "Mock Name";
    private const string Surname = "Mock Surname";
    private const string Password = "Mock Password";
    private const string Icon = "Mock Icon";

    [Fact]
    public void CreateConstructorSuccessfullyWhenValidParametersArePassed()
    {
        var user = new User(PersonalId, Name, Surname, Password, Role.CASHIER);
 
        Assert.Equal("E001", user.PersonalId);
        Assert.True(user.IsFirstLogin);
        Assert.True(user.IsActive);
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void ThrowAnExceptionWhenPersonalIdIsEmpty(string? personalId)
    {
        Assert.Throws<ArgumentException>(() =>
            new User(personalId!, Name, Surname, Password, Role.CASHIER));
    }
    
    [Theory]
    [InlineData("", Surname)]
    [InlineData(Name, "")]
    [InlineData("   ", Surname)]
    public void ThrowAnExceptionWhenNameIsEmpty(string firstName, string lastName)
    {
        Assert.Throws<ArgumentException>(() =>
            new User(PersonalId, firstName, lastName, Password, Role.CASHIER));
    }
    
    [Fact]
    public void IsFirstLoginIsTrueWhenChangedPassword()
    {
        var user = new User(PersonalId, Name, Surname, Password, Role.CASHIER);
 
        user.ChangePassword("newpass");
 
        Assert.False(user.IsFirstLogin);
    }
 
    [Fact]
    public void ThrowAnExceptionWhenNewPasswordIsEmptyAndStaySameFirstLogin()
    {
        var user = new User(PersonalId, Name, Surname, Password, Role.CASHIER);
 
        Assert.Throws<ArgumentException>(() => user.ChangePassword("   "));
        Assert.True(user.IsFirstLogin);
    }
 
    [Fact]
    public void LocationListWillUpdateCorrectlyWhenGiveLocationAccessRun()
    {
        var user = new User(PersonalId, Name, Surname, Password, Role.CASHIER);
        var newLocationId = Guid.NewGuid();
 
        user.GiveLocationAccess(newLocationId);
 
        Assert.Single(new[] { user.Locations.Count });
        Assert.True(user.HasAccessToLocation(newLocationId));
    }
 
    [Fact]
    public void ThrowAnErrorWhenDuplicatesSameLocationForAccess()
    {
        var locationId = Guid.NewGuid();
        var user = new User(PersonalId, Name, Surname, Password, Role.CASHIER);
        user.GiveLocationAccess(locationId);
        Assert.Throws<InvalidOperationException>(() => user.GiveLocationAccess(locationId));
    }
 

    [Fact]
    public void RevokeAccessWillWorkCorrectlyWhenThereAreManyLocations()
    {
        var locationId1 = Guid.NewGuid();
        var locationId2 = Guid.NewGuid();
        var user = new User(PersonalId, Name, Surname, Password, Role.CASHIER);
        user.GiveLocationAccess(locationId2);
 
        user.RevokeLocationAccess(locationId1);
 
        Assert.False(user.HasAccessToLocation(locationId1));
        Assert.True(user.HasAccessToLocation(locationId2));
        Assert.Single(user.Locations);
    }
 
    [Fact]
    public void DoesNothingWhenUserNotAuthorizedForALocationAndRunRevokeAccess()
    {
        var locationId2 = Guid.NewGuid();
        var user = new User(PersonalId, Name, Surname, Password, Role.CASHIER);
        user.GiveLocationAccess(locationId2);
 
        user.RevokeLocationAccess(Guid.NewGuid());
 
        Assert.Single(new[] { user.Locations.Count });
    }
 
    [Fact]
    public void DeactivateUserWillWorkCorrectlyWhenUserIsActive()
    {
        var user = new User(PersonalId, Name, Surname, Password, Role.CASHIER);
 
        user.DeactivateUser();
 
        Assert.False(user.IsActive);
    }
 
    [Fact]
    public void ActivateUserWillWorkCorrectlyWhenUserIsNotActive()
    {
        var user = new User(PersonalId, Name, Surname, Password, Role.CASHIER);
        user.DeactivateUser();
 
        user.ActivateUser();
 
        Assert.True(user.IsActive);
    }
    
}
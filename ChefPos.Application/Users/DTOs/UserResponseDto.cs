using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

namespace ChefPos.Application.Users.DTOs;

public class UserResponseDto
{
    public Guid Id { get; set; }
    public string PersonalId { get; set; } = default!;
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public List<Role> Roles { get; set; }
    public bool IsFirstLogin { get; set; }
    public bool IsActive { get; set; }
    public List<Guid> LocationIds { get; set; } = new();
 
    public static UserResponseDto FromEntity(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            PersonalId = user.PersonalId,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Roles = user.Roles.ToList(),
            IsFirstLogin = user.IsFirstLogin,
            IsActive = user.IsActive,
            LocationIds = user.Locations.Select(l => l.LocationId).ToList()
        };
    }
}

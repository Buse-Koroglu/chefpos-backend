using ChefPos.Domain.Enums;

namespace ChefPos.Application.Users.DTOs;

public class UserLocationRoleDto
{
    public Role Role { get; set; }
    public Guid LocationId { get; set; }
}

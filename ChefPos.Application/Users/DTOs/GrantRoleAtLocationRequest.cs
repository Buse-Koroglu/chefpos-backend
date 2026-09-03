using ChefPos.Domain.Enums;

public class GrantRoleAtLocationRequest
{
    public Role Role { get; set; }
    public Guid LocationId { get; set; }
}

using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<List<UserResponseDto>>
{
    public Role? Role { get; set; }
    public bool IncludeInactive { get; set; }
    
    public GetAllUsersQuery(Role? role, bool includeInactive)
    {
        Role = role;
        IncludeInactive = includeInactive;
    }
}
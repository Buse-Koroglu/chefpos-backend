using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<PagedResult<UserResponseDto>>
{
    public string? SearchTerm { get; }
    public Role? Role { get; }
    public bool? IsActive { get; }
    public Guid? LocationId { get; }
    public int PageNumber { get; }
    public int PageSize { get; }

    public GetAllUsersQuery(string? searchTerm, Role? role, bool? isActive, Guid? locationId, int pageNumber = 1, int pageSize = 20)
    {
        SearchTerm = searchTerm;
        Role = role;
        IsActive = isActive;
        LocationId = locationId;
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize is < 1 or > 100 ? 20 : pageSize;
    }
}
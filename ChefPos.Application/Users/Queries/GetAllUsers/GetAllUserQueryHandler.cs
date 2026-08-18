using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Pagination;
using ChefPos.Application.Users.DTOs;
using ChefPos.Application.Users.Queries.GetAllUsers;
using MediatR;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedResult<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PagedResult<UserResponseDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var (users, totalCount) = await _userRepository.GetAllPagedAsync(
            request.SearchTerm, request.Role, request.IsActive, request.LocationId, request.PageNumber, request.PageSize, cancellationToken);

        return new PagedResult<UserResponseDto>
        {
            Items = users.Select(UserResponseDto.FromEntity).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using ChefPos.Application.Users.Queries.GetAllUsers;
using MediatR;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, List<UserResponseDto>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<List<UserResponseDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);

        var filtered = users.AsEnumerable();

        if (!request.IncludeInactive)
            filtered = filtered.Where(u => u.IsActive);

        if (request.Role.HasValue)
            filtered = filtered.Where(u => u.HasRole(request.Role.Value));

        return filtered.Select(UserResponseDto.FromEntity).ToList();
    }
}
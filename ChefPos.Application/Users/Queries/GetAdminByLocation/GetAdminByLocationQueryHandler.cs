using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Users.Queries.GetAdminByLocation;

public class GetAdminByLocationQueryHandler : IRequestHandler<GetAdminByLocationQuery, UserResponseDto?>
{
    private readonly IUserRepository _userRepository;

    public GetAdminByLocationQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto?> Handle(GetAdminByLocationQuery request, CancellationToken cancellationToken)
    {
        var admin = await _userRepository.GetAdminByLocationAsync(request.LocationId, cancellationToken);

        return admin is null ? null : UserResponseDto.FromEntity(admin);
    }
}

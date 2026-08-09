using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Users.Commands.RemoveRole;

public class RemoveUserRoleCommandHandler : IRequestHandler<RemoveRoleCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;

    public RemoveUserRoleCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserResponseDto> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {request.UserId}");
        user.RemoveRole(request.Role);

        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}
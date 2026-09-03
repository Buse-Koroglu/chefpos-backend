using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Users.Commands.RemoveRole;

public class RemoveUserRoleCommandHandler : IRequestHandler<RemoveRoleCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public RemoveUserRoleCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<UserResponseDto> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.Role != Role.SUPER_ADMIN)
        {
            throw new ValidationException("Bu uç nokta yalnızca süper yönetici rolü içindir; diğer roller için yerleşkeye özel rol kaldırma kullanılmalıdır.");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {request.UserId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN))
        {
            throw new ValidationException("Bu rolü yalnızca süper yönetici kaldırabilir.");
        }

        user.RemoveRole(request.Role);

        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}
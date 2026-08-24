using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;
namespace ChefPos.Application.Users.Commands.RevokeLocationAccess;

public class RevokeLocationAccessCommandHandler : IRequestHandler<RevokeLocationAccessCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public RevokeLocationAccessCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<UserResponseDto> Handle(RevokeLocationAccessCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundException($"Kullanıcı bulunamadı: {request.UserId}");

        if (user.HasRole(Role.ADMIN) || user.HasRole(Role.SUPER_ADMIN))
        {
            var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken);
            if (actingUser is null)
                throw new NotFoundException($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

            if (!actingUser.HasRole(Role.SUPER_ADMIN))
            {
                throw new ValidationException("Bu rolü yalnızca süper yönetici kaldırabilir.");
            }
        }

        user.RevokeLocationAccess(request.LocationId);
        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.Commands.DeactivateUser;
using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public DeactivateUserCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<UserResponseDto> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
            throw new NotFoundException($"Kullanıcı bulunamadı: {request.UserId}");

        var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken);
        if (actingUser is null)
            throw new NotFoundException($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

        if (!actingUser.HasRole(Role.SUPER_ADMIN) &&
            !user.Locations.Select(l => l.LocationId).Any(id => actingUser.Locations.Any(al => al.LocationId == id)))
        {
            throw new ValidationException("Bu kullanıcıyı yönetme yetkiniz yok.");
        }

        user.DeactivateUser();
        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}
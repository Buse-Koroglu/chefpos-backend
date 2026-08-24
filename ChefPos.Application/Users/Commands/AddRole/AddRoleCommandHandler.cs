using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.Commands.AddRole;
using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Enums;
using MediatR;

public class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddRoleCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async Task<UserResponseDto> Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {request.UserId}");

        if (request.Role == Role.ADMIN || request.Role == Role.SUPER_ADMIN)
        {
            var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken)
                .OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");

            if (!actingUser.HasRole(Role.SUPER_ADMIN))
            {
                throw new ValidationException("Bu rolü yalnızca süper yönetici atayabilir.");
            }
        }

        if (request.Role == Role.SUPER_ADMIN)
        {
            var existingSuperAdmin = await _userRepository.GetSuperAdminAsync(cancellationToken);
            if (existingSuperAdmin is not null && existingSuperAdmin.Id != user.Id)
            {
                throw new ValidationException(
                    $"Sistemde zaten bir süper yönetici var: {existingSuperAdmin.FirstName} {existingSuperAdmin.LastName}.");
            }

            if (user.HasRole(Role.ADMIN))
            {
                throw new ValidationException(
                    "Yönetici rolüne sahip bir kullanıcı süper yönetici yapılamaz. Önce yöneticilik rolünü kaldırın.");
            }
        }

        if (request.Role == Role.ADMIN && user.HasRole(Role.SUPER_ADMIN))
        {
            throw new ValidationException("Süper yönetici bir yerleşkenin yöneticisi olamaz.");
        }

        user.AddRole(request.Role);

        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;
using MediatR;

namespace ChefPos.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserCreatedResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IInitialPasswordGenerator  _initialPasswordGenerator;
    private readonly ICurrentUserService _currentUserService;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, IInitialPasswordGenerator initialPasswordGenerator, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _initialPasswordGenerator = initialPasswordGenerator;
        _currentUserService = currentUserService;
    }

    public async Task<UserCreatedResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByPersonalIdAsync(request.PersonalId, cancellationToken);
        if (existing is not null)
        {
            throw new ValidationException("Bu personel ID ile zaten bir kullanıcı mevcut.");
        }

        if (request.Roles.Contains(Role.ADMIN) || request.Roles.Contains(Role.SUPER_ADMIN))
        {
            var actingUser = await _userRepository.GetByIdAsync(_currentUserService.UserId, cancellationToken).OrThrowNotFoundAsync($"Kullanıcı bulunamadı: {_currentUserService.UserId}");
            if (!actingUser.HasRole(Role.SUPER_ADMIN))
            {
                throw new ValidationException("Bu rolü yalnızca süper yönetici atayabilir.");
            }
        }

        if (request.Roles.Contains(Role.ADMIN) && request.Roles.Contains(Role.SUPER_ADMIN))
        {
            throw new ValidationException("Yönetici rolüne sahip bir kullanıcı süper yönetici yapılamaz. Önce yöneticilik rolünü kaldırın.");
        }

        var generatedPassword = _initialPasswordGenerator.Generate(request.FirstName, request.PersonalId);
        var hashedPassword = _passwordHasher.Hash(generatedPassword);

        var user = new User(request.PersonalId, request.FirstName, request.LastName, hashedPassword, request.Roles);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserCreatedResponseDto.FromEntity(user, generatedPassword);
    }
}
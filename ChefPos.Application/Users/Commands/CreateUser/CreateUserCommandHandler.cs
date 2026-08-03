using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using ChefPos.Domain.Entities;
using MediatR;

namespace ChefPos.Application.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserResponseDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByPersonalIdAsync(request.PersonalId, cancellationToken);
        if (existing is not null)
        {
            throw new InvalidOperationException("Bu personel ID ile zaten bir kullanıcı mevcut.");
        }

        var hashedPassword = _passwordHasher.Hash(request.Password);

        var user = new User(request.PersonalId, request.FirstName, request.LastName, hashedPassword, request.Role);

        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveAllChangesAsync(cancellationToken);

        return UserResponseDto.FromEntity(user);
    }
}
using ChefPos.Application.Auth.Commands.DTOs;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using MediatR;

namespace ChefPos.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByPersonalIdAsync(request.PersonalId, cancellationToken);

        if (user is null || !user.IsActive)
            throw new AuthenticationFailedException("Kullanıcı bulunamadı veya pasif.");

        if (!_passwordHasher.Verify(request.Password, user.Password))
            throw new AuthenticationFailedException("Şifre hatalı.");

        var token = _jwtTokenService.GenerateToken(user);

        return new LoginResponseDto { Token = token };
    }
}
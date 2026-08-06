using ChefPos.Application.Auth.Commands.DTOs;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LoginCommandHandler(IUserRepository userRepository,IPasswordHasher passwordHasher,IJwtTokenGenerator jwtTokenGenerator, IRefreshTokenGenerator refreshTokenGenerator, IRefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByPersonalIdAsync(request.PersonalId, cancellationToken);

        if (user is null || !user.IsActive)
            throw new AuthenticationFailedException("Kullanıcı bulunamadı veya pasif.");

        if (!_passwordHasher.Verify(request.Password, user.Password))
            throw new AuthenticationFailedException("Şifre hatalı.");

        var (accessToken, accessTokenExpiresAt) = _jwtTokenGenerator.GenerateToken(user);
        var (rawRefreshToken, refreshTokenHash, refreshTokenExpiresAt) = _refreshTokenGenerator.Generate();
 
        var refreshToken = new Domain.Entities.RefreshToken(user.Id, refreshTokenHash, refreshTokenExpiresAt);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
        await _refreshTokenRepository.SaveAllChangesAsync(cancellationToken);
 
        return new LoginResponseDto
        {
            Token = accessToken,
            ExpiresAt = accessTokenExpiresAt,
            User = UserResponseDto.FromEntity(user),
            RefreshToken = rawRefreshToken,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }
}
using ChefPos.Application.Auth.Commands.DTOs;
using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Users.DTOs;
using MediatR;

namespace ChefPos.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IRefreshTokenGenerator refreshTokenGenerator)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var incomingHash = _refreshTokenGenerator.Hash(request.RawRefreshToken);
        var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(incomingHash, cancellationToken);

        if (existingToken is null)
        {
            throw new AuthenticationFailedException("Geçersiz refresh token.");
        }

        if (existingToken.IsRevoked)
        {
            await _refreshTokenRepository.RevokeAllForUserAsync(existingToken.UserId, cancellationToken);
            await _refreshTokenRepository.SaveAllChangesAsync(cancellationToken);

            throw new AuthenticationFailedException("Bu oturum güvenlik nedeniyle sonlandırıldı, lütfen tekrar giriş yapın.");
        }

        if (existingToken.IsExpired)
        {
            throw new AuthenticationFailedException("Oturum süresi dolmuş, lütfen tekrar giriş yapın.");
        }

        var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken).OrThrowNotFoundAsync("Kullanıcı bulunamadı.");

        if (!user.IsActive)
        {
            throw new AuthenticationFailedException("Bu kullanıcı hesabı pasif durumda.");
        }

        var (accessToken, accessTokenExpiresAt) = _jwtTokenGenerator.GenerateToken(user);
        var (rawRefreshToken, refreshTokenHash, refreshTokenExpiresAt) = _refreshTokenGenerator.Generate();

        var newRefreshToken = new Domain.Entities.RefreshToken(user.Id, refreshTokenHash, refreshTokenExpiresAt);
        await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
        
        existingToken.Revoke(refreshTokenHash);

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
using ChefPos.Application.Common.Interfaces;
using MediatR;

namespace ChefPos.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;

    public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository, IRefreshTokenGenerator refreshTokenGenerator)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenGenerator = refreshTokenGenerator;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var hash = _refreshTokenGenerator.Hash(request.RawRefreshToken);
        var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(hash, cancellationToken);
        if (existingToken is not null && !existingToken.IsRevoked)
        {
            existingToken.Revoke();
            await _refreshTokenRepository.SaveAllChangesAsync(cancellationToken);
        }
    }
}
using ChefPos.Domain.Entities;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    Task RevokeAllForUserAsync(Guid userId, CancellationToken cancellationToken);
    Task SaveAllChangesAsync(CancellationToken cancellationToken);
}
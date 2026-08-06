using System.Security.Cryptography;
using ChefPos.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace ChefPos.Infastructure.Security;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private const int TokenSizeInBytes = 64;

    private readonly JwtSettings _settings;

    public RefreshTokenGenerator(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public (string RawToken, string TokenHash, DateTime ExpiresAt) Generate()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(TokenSizeInBytes);
        var rawToken = Convert.ToBase64String(randomBytes);
        var expiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays);

        return (rawToken, Hash(rawToken), expiresAt);
    }

    public string Hash(string rawToken)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(rawToken);
        var hashBytes = SHA256.HashData(bytes);
        return Convert.ToHexString(hashBytes);
    }
}
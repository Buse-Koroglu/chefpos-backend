using System.Security.Cryptography;
using ChefPos.Application.Common.Interfaces;
using Microsoft.Extensions.Options;

namespace ChefPos.Infrastructure.Security;

public class RefreshTokenGenerator : IRefreshTokenGenerator
{
    private const int TokenSize = 64;

    private readonly JwtSettings _settings;

    public RefreshTokenGenerator(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public (string RawToken, string HashToken, DateTime ExpiresAt) Generate()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(TokenSize); // 64 byte'lık random bir dizi 
        var rawToken = Convert.ToBase64String(randomBytes); // binary veri --> base64 string
        var expiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays); // expire time hesaplaması

        return (rawToken, Hash(rawToken), expiresAt); 
    }

    public string Hash(string rawToken)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(rawToken); // raw token string -> utf8 byte
        var hashBytes = SHA256.HashData(bytes); // utf8 byte'larını hash
        return Convert.ToHexString(hashBytes);  // hash to string
    }
}
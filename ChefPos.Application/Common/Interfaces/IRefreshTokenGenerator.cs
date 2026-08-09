namespace ChefPos.Application.Common.Interfaces;

public interface IRefreshTokenGenerator {
    
    (string RawToken, string TokenHash, DateTime ExpiresAt) Generate();
    string Hash(string rawToken);
}
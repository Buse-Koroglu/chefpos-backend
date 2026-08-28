namespace ChefPos.Application.Common.Interfaces;

public interface IRefreshTokenGenerator {
    
    (string RawToken, string HashToken, DateTime ExpiresAt) Generate();
    string Hash(string rawToken);
}
namespace ChefPos.Infastructure.Security;

public class JwtSettings
{
    public string Issuer { get; set; } = default!; // token'ı kim üretti?
    public string Audience { get; set; } = default!; // token kimin için üretildi?
    public string SecretKey { get; set; } = default!;
    public int ExpiryMinutes { get; set; } = 15; // access token için expiry minute = 15
    public int RefreshTokenExpiryDays { get; set; } = 7;  // refresh token expiry day = 7
}
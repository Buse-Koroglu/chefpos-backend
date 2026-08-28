using System.Text.Json.Serialization;
using ChefPos.Application.Users.DTOs;

namespace ChefPos.Application.Auth.Commands.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public UserResponseDto User { get; set; } = default!;
    [JsonIgnore] // client tarafında gözükmemesi için
    public string RefreshToken { get; set; } = default!;
    [JsonIgnore]  // client tarafında gözükmemesi için
    public DateTime RefreshTokenExpiresAt { get; set; }
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ChefPos.Infastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration; 

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiresAt)  GenerateToken(User user)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var secret = jwtSection["Secret"]!;
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"]!);
        
        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        // claim içerisinde user id, personel id ve kullanıcı rolleri bulunuyor. 
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.PersonalId),
        };  
        claims.AddRange(user.Roles.Select(r => new Claim(ClaimTypes.Role, r.ToString())));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // token'ı bu key ve sha256 algoritması ile imzalayacak.
        
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return (tokenString, expiresAt);
    }
}
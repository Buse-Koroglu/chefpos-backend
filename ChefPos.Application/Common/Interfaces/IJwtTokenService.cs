using ChefPos.Domain.Entities;

namespace ChefPos.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
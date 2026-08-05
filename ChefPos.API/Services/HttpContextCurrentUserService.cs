using System.Security.Claims;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;

namespace ChefPos.API.Services;

public class HttpContextCurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextCurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            var userIdValue = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdValue) || !Guid.TryParse(userIdValue, out var userId))
            {
                throw new AuthenticationFailedException("Geçerli kullanıcı bilgisi bulunamadı.");
            }

            return userId;
        }
    }
}

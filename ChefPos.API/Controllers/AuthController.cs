using ChefPos.Application.Auth.Commands.ChangePassword;
using ChefPos.Application.Auth.Commands.Login;
using ChefPos.Application.Auth.Commands.RefreshToken;
using ChefPos.Application.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName = "refreshToken";

    private readonly IMediator _mediator;
    
    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginCommand command, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);

        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh(CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(RefreshTokenCookieName, out var rawRefreshToken) || string.IsNullOrWhiteSpace(rawRefreshToken))
        {
            throw new AuthenticationFailedException("Refresh token bulunamadı.");
        }

        var command = new RefreshTokenCommand(rawRefreshToken);
        var result = await _mediator.Send(command, cancellationToken);

        SetRefreshTokenCookie(result.RefreshToken, result.RefreshTokenExpiresAt);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        try
        {
            if (Request.Cookies.TryGetValue(RefreshTokenCookieName, out var rawRefreshToken) && !string.IsNullOrWhiteSpace(rawRefreshToken))
            {
                await _mediator.Send(new LogoutCommand(rawRefreshToken), cancellationToken);
            }
        }
        finally
        {
            DeleteRefreshTokenCookie();
        }

        return Ok();
    }

    private void SetRefreshTokenCookie(string rawRefreshToken, DateTime expiresAt)
    {
        Response.Cookies.Append(RefreshTokenCookieName, rawRefreshToken, GetCookieOptions(expiresAt));
    }

    private void DeleteRefreshTokenCookie()
    {
        Response.Cookies.Delete(RefreshTokenCookieName, GetCookieOptions(DateTime.UtcNow.AddDays(-1)));
    }

    private static CookieOptions GetCookieOptions(DateTime expiresAt)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = expiresAt,
            Path = "/api/auth"
        };
    }
}
using ChefPos.Application.Auth.Commands.DTOs;
using MediatR;

namespace ChefPos.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<LoginResponseDto>
{
    public string RawRefreshToken { get; set; } = default!;

    public RefreshTokenCommand(string rawRefreshToken)
    {
        RawRefreshToken = rawRefreshToken;
    }
}
using ChefPos.Application.Auth.Commands.DTOs;
using MediatR;

namespace ChefPos.Application.Auth.Commands.Login;

public class LoginCommand : IRequest<LoginResponseDto>
{
    public string PersonalId { get; set; } = default!;
    public string Password { get; set; } = default!;
}
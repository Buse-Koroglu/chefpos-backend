using MediatR;

namespace ChefPos.Application.Auth.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest
{
    public string NewPassword { get; set; } = default!;
}
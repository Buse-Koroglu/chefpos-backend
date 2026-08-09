using MediatR;

public class LogoutCommand : IRequest
{
    public string RawRefreshToken { get; set; } = default!;

    public LogoutCommand(string rawRefreshToken)
    {
        RawRefreshToken = rawRefreshToken;
    }
}
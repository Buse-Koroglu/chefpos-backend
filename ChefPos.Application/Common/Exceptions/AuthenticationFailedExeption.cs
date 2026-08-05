namespace ChefPos.Application.Common.Exceptions;

public class AuthenticationFailedException : AppException
{
    public override int StatusCode => 401;
 
    public AuthenticationFailedException(string message) : base(message)
    {
    }
}
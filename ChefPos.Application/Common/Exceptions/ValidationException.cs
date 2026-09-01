namespace ChefPos.Application.Common.Exceptions;

public class ValidationException : AppException
{
    public override int StatusCode => 400;

    public ValidationException(string message, string? errorCode = null) : base(message, errorCode)
    {
    }
}
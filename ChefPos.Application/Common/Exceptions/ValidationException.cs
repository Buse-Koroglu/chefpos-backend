namespace ChefPos.Application.Common.Exceptions;

public class ValidationException : AppException
{
    public override int StatusCode => 400;

    public ValidationException(string message) : base(message)
    {
    }
}
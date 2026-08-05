namespace ChefPos.Application.Common.Exceptions;

public class NotFoundException : AppException
{
    public override int StatusCode => 404;

    public NotFoundException(string message) : base(message)
    {
    }
}
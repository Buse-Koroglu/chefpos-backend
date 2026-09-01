namespace ChefPos.Application.Common.Exceptions;

public abstract class AppException : Exception
{
    public abstract int StatusCode { get; }
    public string? ErrorCode { get; }

    protected AppException(string message, string? errorCode = null) : base(message)
    {
        ErrorCode = errorCode;
    }
}
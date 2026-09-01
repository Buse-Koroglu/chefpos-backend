using ChefPos.Application.Common.Exceptions;

namespace ChefPos.API.Middleware;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "İşlenmeyen bir hata oluştu: {Message}", exception.Message);

        var (statusCode, title) = MapException(exception);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = httpContext.Request.Path
        };

        if (exception is AppException { ErrorCode: not null } appException)
        {
            problemDetails.Extensions["errorCode"] = appException.ErrorCode;
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        AppException appException => (appException.StatusCode, GetTitle(appException.StatusCode)),
        ArgumentException => (StatusCodes.Status400BadRequest, "Geçersiz istek"),
        InvalidOperationException => (StatusCodes.Status400BadRequest, "İş kuralı ihlali"),
        UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Yetkisiz işlem"),
        KeyNotFoundException => (StatusCodes.Status404NotFound, "Kayıt bulunamadı"),
        _ => (StatusCodes.Status500InternalServerError, "Beklenmeyen bir hata oluştu")
    };

    private static string GetTitle(int statusCode) => statusCode switch
    {
        StatusCodes.Status400BadRequest => "Geçersiz istek",
        StatusCodes.Status401Unauthorized => "Kimlik doğrulama başarısız",
        StatusCodes.Status403Forbidden => "Yetkisiz işlem",
        StatusCodes.Status404NotFound => "Kayıt bulunamadı",
        _ => "Beklenmeyen bir hata oluştu"
    };
}